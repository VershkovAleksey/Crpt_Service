using System.Text;
using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using Database.Context;
using Database.Entities.CreateSetRequest;
using Domain.Models.Crpt.Marking.Dto;
using Domain.Models.Crpt.Marking.Enums;
using Domain.Models.Crpt.Marking.Request;
using Domain.Models.Crpt.Marking.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BL.Services;

public sealed class MarkingService(
    ILogger<MarkingService> logger,
    ICrptHttpClient crptHttpClient,
    CrptContext dbContext,
    ICurrentUserService currentUserService) : IMarkingService
{
    private readonly ILogger<MarkingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly CrptContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly ICurrentUserService _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    private readonly ICrptHttpClient _crptHttpClient = crptHttpClient ?? throw new ArgumentNullException(nameof(crptHttpClient));

    public async Task<List<MarkingListDto>> GetIdentificationCodesAsync(
        string token,
        IEnumerable<string> gtins,
        CancellationToken cancellationToken = default)
    {
        // Валидация входных параметров
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Токен пустой или null в {MethodName}", nameof(GetIdentificationCodesAsync));
            throw new ArgumentNullException(nameof(token));
        }
        if (gtins == null || !gtins.Any())
        {
            _logger.LogInformation("Передан пустой список GTIN в {MethodName}", nameof(GetIdentificationCodesAsync));
            return new List<MarkingListDto>();
        }

        var cises = await GetCisesAsync(token, gtins, cancellationToken);
        
        if (cises == null || !cises.Any())
        {
            _logger.LogInformation("Получен пустой результат от GetCisesAsync для GTIN: {Gtins}", string.Join(", ", gtins));
            return new List<MarkingListDto>();
        }

        // Объединяем результаты для наборов и товаров
        var result = GetMarkingLists(cises, new[] { "BUNDLE", "GROUP", "SET" }, "SET")
            .Concat(GetMarkingLists(cises, new[] { "UNIT" }, "UNIT"))
            .ToList();

        return result;
    }

    public async Task<List<GetCisesResult>> GetCisesAsync(string token, IEnumerable<string> gtins, CancellationToken cancellationToken = default)
    {
        // Валидация входных параметров
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Токен пустой или null в {MethodName}", nameof(GetCisesAsync));
            throw new ArgumentNullException(nameof(token));
        }
        
        if (gtins == null || !gtins.Any())
        {
            _logger.LogInformation("Передан пустой список GTIN в {MethodName}", nameof(GetCisesAsync));
            return new List<GetCisesResult>();
        }

        var allCises = new List<GetCisesResult>();
        
        var request = MapRequest(gtins);
        
        var cisesPage = await _crptHttpClient.GetCisesAsync(token, request, cancellationToken);

        if (cisesPage?.Result == null)
        {
            _logger.LogInformation("Получен пустой результат от API для GTIN: {Gtins}", string.Join(", ", gtins));
            return allCises;
        }

        // Фильтрация и добавление результатов первой страницы
        allCises.AddRange(cisesPage.Result.Where(x => x.Status == "APPLIED"));

        // Пагинация
        while (!cisesPage.IsLastPage)
        {
            var lastResult = cisesPage.Result[^1]; // Используем индексатор для последнего элемента
            
            var pagination = new Pagination
            {
                LastEmissionDate = lastResult.EmissionDate,
                Sgtin = lastResult.Sgtin,
                Direction = 0
            };

            cisesPage = await _crptHttpClient.GetCisesAsync(token, MapPaginationRequest(pagination, gtins), cancellationToken);
            
            if (cisesPage?.Result != null)
            {
                allCises.AddRange(cisesPage.Result.Where(x => x.Status == "APPLIED"));
            }
        }

        return allCises;
    }
    
    public async Task<CreateDocumentBodyRequest> CreateSetsAsync(string token, int userId,
        CancellationToken cancellationToken = default)
    {
        // 1. Получение уникальных GTIN из CreateSetRequests
        var currentUserId = _currentUserService.CurrentUser.Id;

        var setGtins = await _dbContext.CreateSetRequests
            .Where(x => x.UserId == currentUserId && x.Status == (int)CreateSetStatus.Proccessed)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!setGtins.Any())
        {
            _logger.LogInformation("Нет запросов на создание наборов для пользователя {UserId}", currentUserId);
            throw new InvalidOperationException("Нет запросов на создание наборов.");
        }

        // 2. Получение Sets
        var setGtinsSet = setGtins
            .Select(x => x.Gtin)
            .ToHashSet();

        var setsToCreate = await _dbContext.Sets
            .Where(x => setGtinsSet.Contains(x.Gtin))
            .Select(x => new SetToCreateDto()
            {
                Gtin = x.Gtin,
                Id = x.Id
            })
            .ToListAsync(cancellationToken);

        var setIds = setsToCreate.Select(x => x.Id).ToHashSet();

        // 3. Получение GTIN из Units с фильтрацией по SetIds в базе
        var unitGtins = await _dbContext.Units
            .Where(x => x.UserId == currentUserId && x.SetIds.Any(id => setIds.Contains(id)))
            .Select(x => x.Gtin)
            .Distinct()
            .ToListAsync(cancellationToken);

        var gtinsToCreate = setGtinsSet.Union(unitGtins).ToList();

        // 4. Получение кодов идентификации
        var cises = await GetIdentificationCodesAsync(token, gtinsToCreate, cancellationToken);

        // 5. Фильтрация кодов
        var setsCisesList = cises
            .Where(x => x.CisesType == "SET" && setGtinsSet.Contains(x.Gtin.Remove(0, 1)))
            .ToList();

        if (!setsCisesList.Any())
        {
            _logger.LogWarning("Не найдены коды маркировки для наборов: {Gtins}", string.Join(", ", setGtins));
            throw new InvalidOperationException("Не найдены коды маркировки для наборов.");
        }

        var unitsCisesList = cises
            .Where(x => x.CisesType == "UNIT" && unitGtins.Contains(x.Gtin.Remove(0, 1)))
            .ToList();

        if (!unitsCisesList.Any())
        {
            _logger.LogWarning("Не найдены коды маркировки для товаров: {Gtins}", string.Join(", ", unitGtins));
            throw new InvalidOperationException("Не найдены коды маркировки для товаров.");
        }

        // 6. Логирование после проверок
        _logger.LogInformation("GTIN наборов для создания: {Gtins}", string.Join(", ", setGtins.Select(x => x.Gtin)));

        // 7. Формирование агрегации
        var aggregationUnits = await GetAggregationUnitsAsync(currentUserId, setGtins, setsCisesList, setsToCreate, unitsCisesList, cancellationToken);

        // 8. Формирование запроса
        var requestBody = new CreateSetsRequest
        {
            AggregationUnits = aggregationUnits,
            ParticipantId = _currentUserService.CurrentUser.Inn
        };

        return new CreateDocumentBodyRequest
        {
            ProductDocument = JsonConvert.SerializeObject(requestBody),
            Signature = string.Empty
        };
    }

    public async Task SignDataAsync(string token, int userId, CreateDocumentBodyRequest createDocumentBodyRequest,
        CancellationToken cancellationToken = default)
    {
        createDocumentBodyRequest.ProductDocument =
            Convert.ToBase64String(Encoding.UTF8.GetBytes(createDocumentBodyRequest.ProductDocument));

        var setId = await _crptHttpClient.CreateSetsAsync(token, createDocumentBodyRequest, cancellationToken);

        if (setId is not null)
        {
            var createdSets = _dbContext.CreateSetRequests
                .Where(x => x.UserId == userId && x.Status == (int)CreateSetStatus.Proccessed)
                .ToList();

            createdSets.ForEach(x =>
            {
                x.Status = (int)CreateSetStatus.Created;
                x.Response = setId;
            });

            _dbContext.CreateSetRequests.UpdateRange(createdSets);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<string?> CheckCisesAsync(
        List<CreateSetRequestEntity> createRequests,
        List<MarkingListDto> setsCisesList,
        List<SetToCreateDto> setsToCreate,
        List<MarkingListDto> unitsCisesList,
        int userId,
        CancellationToken cancellationToken = default)
    {
        // Создаём словари для быстрого доступа
        var setsCisesDict = setsCisesList.ToDictionary(x => x.Gtin, x => x.Cises.Count);
        var setsToCreateDict = setsToCreate.ToDictionary(x => x.Gtin, x => x.Id);

        // Загружаем Units один раз
        var unitsBySetId = await _dbContext.Units
            .Where(x => x.UserId == userId)
            .Select(x => new { x.Gtin, x.SetIds })
            .ToListAsync(cancellationToken);

        var errorMessages = new StringBuilder();

        foreach (var request in createRequests)
        {
            // Проверка кодов для наборов
            string setGtinKey = "0" + request.Gtin;
            if (!setsCisesDict.TryGetValue(setGtinKey, out int cisesCount) || cisesCount < request.Count)
            {
                int missingCount = request.Count - (setsCisesDict.TryGetValue(setGtinKey, out cisesCount) ? cisesCount : 0);
                errorMessages.AppendLine($"Не хватает {missingCount} кодов маркировки для набора GTIN: {request.Gtin}");
            }

            // Получение setId
            if (!setsToCreateDict.TryGetValue(request.Gtin, out int setId))
            {
                errorMessages.AppendLine($"Не найден набор с GTIN: {request.Gtin}");
                continue;
            }

            // Получение GTIN товаров для набора
            var unitsOfSet = unitsBySetId
                .Where(x => x.SetIds.Contains(setId))
                .Select(x => x.Gtin)
                .Distinct()
                .ToList();

            // Проверка кодов для товаров
            var cisesOfUnit = unitsCisesList
                .Where(x => unitsOfSet.Contains(x.Gtin.Remove(0, 1)))
                .ToList();

            foreach (var unit in cisesOfUnit)
            {
                if (unit.Cises.Count < request.Count)
                {
                    errorMessages.AppendLine(
                        $"Не хватает {request.Count - unit.Cises.Count} кодов маркировки для товара GTIN: {unit.Gtin.Remove(0, 1)}");
                }
            }
        }

        return errorMessages.Length > 0 ? errorMessages.ToString() : null;
    }

    private async Task<List<AggregationUnit>> GetAggregationUnitsAsync(
        int userId,
        List<CreateSetRequestEntity> createRequests,
        List<MarkingListDto> setsCisesList,
        List<SetToCreateDto> setsToCreate,
        List<MarkingListDto> unitsCisesList,
        CancellationToken cancellationToken = default)
    {
        // Проверка наличия кодов
        string? errorMessage = await CheckCisesAsync(createRequests, setsCisesList, setsToCreate, unitsCisesList,
            userId, cancellationToken);
        
        if (errorMessage != null)
        {
            _logger.LogError("Ошибка при создании AggregationUnits: {ErrorMessage}", errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        // Создаём словари для быстрого доступа
        var setsCisesDict = setsCisesList.ToDictionary(x => x.Gtin, x => x.Cises.Select(c => c.Cis).ToList());
        var setsToCreateDict = setsToCreate.ToDictionary(x => x.Gtin, x => x.Id);

        // Загружаем Units один раз
        var unitsBySetId = await _dbContext.Units
            .Where(x => x.UserId == userId)
            .Select(x => new { x.Gtin, x.SetIds })
            .ToListAsync(cancellationToken);

        var aggregationUnits = new List<AggregationUnit>();

        foreach (var request in createRequests)
        {
            // Получение setId
            if (!setsToCreateDict.TryGetValue(request.Gtin, out int setId))
            {
                _logger.LogWarning("Не найден набор с GTIN: {Gtin}", request.Gtin);
                continue;
            }

            // Получение GTIN товаров для набора
            var unitsOfSet = unitsBySetId
                .Where(x => x.SetIds.Contains(setId))
                .Select(x => x.Gtin)
                .Distinct()
                .ToList();

            // Получение кодов для товаров
            var cisesOfUnit = unitsCisesList
                .Where(x => unitsOfSet.Contains(x.Gtin.Remove(0, 1)))
                .ToList();

            // Получение кодов для набора
            string setGtinKey = "0" + request.Gtin;
            if (!setsCisesDict.TryGetValue(setGtinKey, out var cisesOfSet))
            {
                _logger.LogWarning("Не найдены коды для набора GTIN: {Gtin}", request.Gtin);
                continue;
            }

            // Создание AggregationUnit
            for (int i = 0; i < request.Count && i < cisesOfSet.Count; i++)
            {
                var oneSet = new AggregationUnit
                {
                    UnitSerialNumber = cisesOfSet[i],
                    Sntins = cisesOfUnit
                        .Where(x => i < x.Cises.Count)
                        .Select(x => x.Cises[i].Cis)
                        .ToArray()
                };
                aggregationUnits.Add(oneSet);
            }
        }

        return aggregationUnits;
    }
    
    private IEnumerable<MarkingListDto> GetMarkingLists(
        List<GetCisesResult> cises,
        string[] packageTypes,
        string cisesType)
    {
        var filteredCises = cises
            .Where(x => packageTypes.Contains(x.GeneralPackageType))
            .GroupBy(x => x.Gtin)
            .Select(g => new
            {
                Cises = g.ToList(),
                Gtin = g.Key
            })
            .ToList();

        if (!filteredCises.Any())
        {
            _logger.LogWarning("Не найдены элементы с типом {CisesType} в {MethodName}", cisesType, nameof(GetMarkingLists));
        }

        return filteredCises.Select(group => new MarkingListDto
        {
            Cises = group.Cises,
            CisesType = cisesType,
            Gtin = group.Gtin
        });
    }

    private GetCisesRequest MapRequest(IEnumerable<string> gtins)
    {
        return new GetCisesRequest
        {
            Filter = new CisesFilter
            {
                Gtins = MapGtins(gtins)
            }
        };
    }

    private GetCisesRequest MapPaginationRequest(Pagination pagination, IEnumerable<string> gtins)
    {
        return new GetCisesRequest
        {
            Filter = new CisesFilter
            {
                Gtins = MapGtins(gtins)
            },
            Pagination = pagination
        };
    }

    private string[] MapGtins(IEnumerable<string> gtins)
    {
        if (gtins == null)
            throw new ArgumentNullException(nameof(gtins));

        return gtins.Select(x => "0" + x ?? throw new ArgumentNullException(nameof(gtins), "GTIN не может быть null"))
            .ToArray();
    }
}