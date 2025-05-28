using System.Collections;
using System.Text;
using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using Database.Context;
using Database.Entities.CreateSetRequest;
using Database.Entities.Sets;
using Domain.Models.Crpt.Marking.Dto;
using Domain.Models.Crpt.Marking.Enums;
using Domain.Models.Crpt.Marking.Request;
using Domain.Models.Crpt.Marking.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BL.Services;

public sealed class MarkingService(
    ILogger<MarkingService> logger,
    ICrptHttpClient crptHttpClient,
    CrptContext dbContext,
    IAuthService authService,
    ICurrentUserService currentUserService) : IMarkingService
{
    private readonly ILogger<MarkingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly CrptContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly ICurrentUserService _currentUserService =
        currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

    private readonly ICrptHttpClient _crptHttpClient =
        crptHttpClient ?? throw new ArgumentNullException(nameof(crptHttpClient));

    private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));

    public async Task<List<MarkingListDto>?> GetIdentificationCodesAsync(string token, IEnumerable<string> gtins,
        CancellationToken cancellationToken = default)
    {
        var cises = await GetCisesAsync(token, gtins, cancellationToken);

        var result = new List<MarkingListDto>();

        result.AddRange(GetSets(cises));

        result.AddRange(GetUnits(cises));

        return result;
    }

    private IEnumerable<MarkingListDto> GetSets(List<GetCisesResult> cises)
    {
        var sets = cises.Where(x =>
            x.GeneralPackageType is "BUNDLE" or "GROUP" or "SET").ToList();

        if (sets.Count == 0)
        {
            _logger.LogError("{ClassName}.{MethodName}: No sets found", nameof(MarkingService),
                nameof(GetIdentificationCodesAsync));
        }

        var setGtins = sets.Select(x => x.Gtin).Distinct();

        var sortedSetsByGtinsList = setGtins.Select(gtin => sets.Where(x => x.Gtin == gtin).ToList()).ToList();

        return sortedSetsByGtinsList.Select(setList => new MarkingListDto()
        {
            Cises = setList, CisesType = setList.First().GeneralPackageType, Gtin = setList.First().Gtin!,
        });
    }

    private IEnumerable<MarkingListDto> GetUnits(List<GetCisesResult> cises)
    {
        var units = cises.Where(x => x.GeneralPackageType == "UNIT").ToList();

        if (units.Count == 0)
        {
            _logger.LogError("{ClassName}.{MethodName}: No units found", nameof(MarkingService),
                nameof(GetIdentificationCodesAsync));
        }

        var gtins = units.Select(x => x.Gtin).Distinct();

        var sortedUnitsByGtinsList = gtins.Select(gtin => units.Where(x => x.Gtin == gtin).ToList()).ToList();

        return sortedUnitsByGtinsList.Select(unitsList => new MarkingListDto()
            { Cises = unitsList, CisesType = unitsList.First().GeneralPackageType, Gtin = unitsList.First().Gtin! });
    }

    private async Task<List<GetCisesResult>> GetCisesAsync(string token, IEnumerable<string> gtins,
        CancellationToken cancellationToken)
    {
        var cisesFirstPage = await _crptHttpClient.GetCisesAsync(token, MapRequest(gtins), cancellationToken);

        if (cisesFirstPage is null || cisesFirstPage.Result.Length == 0)
        {
            return null;
        }

        var pagination = new Pagination()
        {
            LastEmissionDate =
                cisesFirstPage.Result.Last(x => !string.IsNullOrWhiteSpace(x.EmissionDate)).EmissionDate ??
                string.Empty,
            Sgtin = cisesFirstPage.Result.Last().Sgtin,
            Direction = 0
        };

        var paginatedCisesZero =
            await _crptHttpClient.GetCisesAsync(token, MapPaginationRequest(pagination, gtins),
                cancellationToken);

        pagination.Direction = 1;

        var paginatedCisesOne = await _crptHttpClient.GetCisesAsync(token, MapPaginationRequest(pagination, gtins),
            cancellationToken);

        var allCises = cisesFirstPage.Result.Where(x => x.Status == "APPLIED").ToList();

        if (paginatedCisesZero is not null)
        {
            allCises = allCises.Concat(paginatedCisesZero.Result.Where(x => x.Status == "APPLIED")).ToList();
        }

        if (paginatedCisesOne is not null)
        {
            allCises = allCises.Concat(paginatedCisesOne.Result.Where(x => x.Status == "APPLIED")).ToList();
        }

        return allCises;
    }

    private bool FindUnits(IEnumerable<int> unitSetIds, IEnumerable<int> setIds)
    {
        return setIds.Any(unitSetIds.Contains);
    }

    public async Task<CreateDocumentBodyRequest> CreateSetsAsync(string token, int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var createRequests = _dbContext.CreateSetRequests
                .Where(x => x.UserId == _currentUserService.CurrentUser.Id &&
                            x.Status == (int)CreateSetStatus.Proccessed)
                .ToList();

            var setGtinsToCreate = createRequests
                .Select(x => x.Gtin)
                .Distinct()
                .ToList();

            var setsToCreate = _dbContext.Sets
                .Where(x => setGtinsToCreate.Contains(x.Gtin))
                .ToList();

            var unitGtinsToCreate = _dbContext.Units
                .Where(x => x.UserId == _currentUserService.CurrentUser.Id)
                .ToList()
                .Where(x => FindUnits(x.SetIds, setsToCreate.Select(x => x.Id)))
                .Select(x => x.Gtin)
                .Distinct()
                .ToList();

            var gtinsToCreate = setGtinsToCreate.Concat(unitGtinsToCreate).ToList();

            var cises = await GetIdentificationCodesAsync(token, gtinsToCreate, cancellationToken);

            //_logger.LogInformation("Cises: {cises}", JsonConvert.SerializeObject(cises));

            _logger.LogInformation("Gtins of sets to create: {gtins}", JsonConvert.SerializeObject(setGtinsToCreate));


            var setsCisesList = cises
                .Where(x => x.CisesType == "SET" && setGtinsToCreate.Contains(x.Gtin.Remove(0, 1)))
                .ToList();

            if (setsCisesList.Count == 0)
            {
                throw new Exception(
                    $"Не найдены коды маркировки для наборов:{JsonConvert.SerializeObject(setGtinsToCreate)}");
            }

            var unitsCisesList = cises
                .Where(x => x.CisesType == "UNIT" && unitGtinsToCreate.Contains(x.Gtin.Remove(0, 1)))
                .ToList();

            if (unitsCisesList.Count == 0)
            {
                throw new Exception(
                    $"Не найдены коды маркировки для товаров:{JsonConvert.SerializeObject(unitGtinsToCreate)}");
            }

            var aggregationUnits = GetAggregationUnits(_currentUserService.CurrentUser.Id, createRequests,
                setsCisesList, setsToCreate,
                unitsCisesList);


            var requestBody = new CreateSetsRequest()
            {
                AggregationUnits = aggregationUnits,
                ParticipantId = _currentUserService.CurrentUser.Inn
            };

            var requestSerialized = JsonConvert.SerializeObject(requestBody);

            return new CreateDocumentBodyRequest()
            {
                ProductDocument = requestSerialized,
                Signature = string.Empty
            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private List<AggregationUnit> GetAggregationUnits(int userId, List<CreateSetRequestEntity> createRequests,
        List<MarkingListDto> setsCisesList, List<SetEntity> setsToCreate, List<MarkingListDto> unitsCisesList)
    {
        var aggregationUnits = new List<AggregationUnit>();

        foreach (var createSetRequest in createRequests)
        {
            for (var i = 0; i < createSetRequest.Count; i++)
            {
                var cisesOfSet = (setsCisesList
                        .FirstOrDefault(x => x.Gtin == "0" + createSetRequest.Gtin)
                        ?.Cises!)
                    .Select(x => x.Cis)
                    .ToList();

                var setId = setsToCreate.FirstOrDefault(x => x.Gtin == createSetRequest.Gtin)?.Id;

                var unitsOfSet = _dbContext.Units
                    .Where(x => x.UserId == userId && x.SetIds.Contains(setId.Value))
                    .Select(x => x.Gtin)
                    .ToList();

                var cisesOfUnit = unitsCisesList
                    .Where(x => unitsOfSet.Contains(x.Gtin.Remove(0, 1)))
                    .ToList();

                var oneSet = new AggregationUnit()
                {
                    UnitSerialNumber = cisesOfSet[i],
                    Sntins = cisesOfUnit.Select(x => x.Cises[i].Cis).ToArray(),
                };

                aggregationUnits.Add(oneSet);
            }
        }

        return aggregationUnits;
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

    private GetCisesRequest MapRequest(IEnumerable<string> gtins)
    {
        var gtin = gtins.Select(x => "0" + x);
        var request = new GetCisesRequest
        {
            Filter = new CisesFilter()
            {
                Gtins = gtin.ToArray()
            }
        };

        return request;
    }

    private GetCisesRequest MapPaginationRequest(Pagination pagination, IEnumerable<string> gtins)
    {
        var gtin = gtins.Select(x => "0" + x);
        var request = new GetCisesRequest
        {
            Filter = new CisesFilter()
            {
                Gtins = gtin.ToArray()
            },
            Pagination = pagination
        };

        return request;
    }
}