using System.Text;
using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using Database.Context;
using Domain.Models.Crpt.Marking.Dto;
using Domain.Models.Crpt.Marking.Enums;
using Domain.Models.Crpt.Marking.Request;
using Domain.Models.NationalCatalog.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BL.Services;

public sealed class MarkingService(
    ILogger<MarkingService> logger,
    ICrptHttpClient crptHttpClient,
    CrptContext dbContext,
    IAuthService authService) : IMarkingService
{
    private readonly ILogger<MarkingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly CrptContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly ICrptHttpClient _crptHttpClient =
        crptHttpClient ?? throw new ArgumentNullException(nameof(crptHttpClient));

    private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));

    public async Task<List<MarkingListDto>?> GetIdentificationCodesAsync(string token,
        CancellationToken cancellationToken = default)
    {
        var cises = await _crptHttpClient.GetCisesAsync(token, MapRequest(), cancellationToken);

        if (cises is null || cises.Result.Length == 0)
        {
            return null;
        }

        var pagination = new Pagination()
        {
            LastEmissionDate = cises.Result.Last(x => !string.IsNullOrWhiteSpace(x.EmissionDate)).EmissionDate ??
                               string.Empty,
            Sgtin = cises.Result.Last().Sgtin,
            Direction = 0
        };

        var paginatedCises =
            await _crptHttpClient.GetCisesAsync(token, MapPaginationRequest(pagination),
                cancellationToken);

        var ki = cises.Result.ToList();

        if (paginatedCises is not null)
        {
            ki = ki.Concat(paginatedCises.Result).ToList();
        }

        var sets = ki.Where(x =>
            x.GeneralPackageType is "BUNDLE" or "GROUP" or "SET").ToList();
        var units = ki.Where(x => x.GeneralPackageType == "UNIT").ToList();

        if (sets.Count == 0)
        {
            _logger.LogError("{ClassName}.{MethodName}: No sets found", nameof(MarkingService),
                nameof(GetIdentificationCodesAsync));
        }

        if (units.Count == 0)
        {
            _logger.LogError("{ClassName}.{MethodName}: No units found", nameof(MarkingService),
                nameof(GetIdentificationCodesAsync));
        }

        var gtins = units.Select(x => x.Gtin).Distinct();

        var sortedUnitsByGtinsList = gtins.Select(gtin => units.Where(x => x.Gtin == gtin).ToList()).ToList();

        var result = new List<MarkingListDto>
        {
            new()
            {
                CisesType = sets.First().GeneralPackageType,
                Cises = sets.ToList(),
                Gtin = sets.First().Gtin,
            }
        };

        result.AddRange(sortedUnitsByGtinsList.Select(unitsList => new MarkingListDto()
            { Cises = unitsList, CisesType = unitsList.First().GeneralPackageType, Gtin = unitsList.First().Gtin! }));

        return result;
    }

    private bool FindUnits(IEnumerable<int> unitSetIds, IEnumerable<int> setIds)
    {
        return setIds.Any(unitSetIds.Contains);
    }

    public async Task<CreateDocumentBodyRequest> CreateSetsAsync(string token, int userId,
        CancellationToken cancellationToken = default)
    {
        var cises = await GetIdentificationCodesAsync(token, cancellationToken);

        var setGtinsToCreate = _dbContext.CreateSetRequests
            .Where(x => x.UserId == userId)
            .Select(x => "0" + x.Gtin)
            .Distinct()
            .ToList();

        var setsToCreate = _dbContext.Sets
            .Where(x => setGtinsToCreate.Contains("0" + x.Gtin))
            .ToList();

        var unitsFromDb = _dbContext.Units
            .Where(x => x.UserId == userId)
            .ToList();
        //.Where(x => x.SetIds != null && x.SetIds.Count != 0 && FindUnits(x.SetIds, setIdsToCreate))
        //.Select(x => x.Gtin);

        var setsCisesList = cises
            .Where(x => x.CisesType == "SET" && setGtinsToCreate.Contains(x.Gtin))
            .ToList();

        if (setsCisesList.Count == 0)
        {
            throw new Exception($"Не найдены коды маркировки для наборов:{setGtinsToCreate}");
        }

        var unitsCisesList = cises
            .Where(x => x.CisesType != "SET" /*&& unitGtinsToCreate.Contains(x.Gtin)*/)
            .ToList();

        var aggregationUnits = new List<AggregationUnit>();

        for (var i = 0; i < setsToCreate.Count; i++)
        {
            var cisesOfSet = setsCisesList
                .Where(x => x.Gtin == "0" + setsToCreate[i].Gtin)
                .ToArray();
            var unitsOfSet = unitsFromDb
                .Where(x => x.UserId == userId && x.SetIds.Contains(setsToCreate[i].Id)).Select(x => x.Gtin)
                .ToList();
            var cisesOfUnit = unitsCisesList
                .Where(x => unitsOfSet.Contains(x.Gtin.Remove(0, 1)))
                .ToList();
            var oneSet = new AggregationUnit()
            {
                UnitSerialNumber = cisesOfSet[i].Cises[i].Cis,
                Sntins = cisesOfUnit.Select(x => x.Cises[i].Cis).ToArray(),
            };

            aggregationUnits.Add(oneSet);
        }


        var requestBody = new CreateSetsRequest()
        {
            AggregationUnits = aggregationUnits,
            ParticipantId = "212702137805" //TODO:Добавить User.Inn
        };

        var requestSerialized = JsonConvert.SerializeObject(requestBody);

        return new CreateDocumentBodyRequest()
        {
            ProductDocument = requestSerialized,
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

            createdSets.All(x => x.Status == (int)CreateSetStatus.Created);
            
            _dbContext.CreateSetRequests.UpdateRange(createdSets);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private GetCisesRequest MapRequest()
    {
        var request = new GetCisesRequest
        {
            Filter = new CisesFilter()
            {
            }
        };

        return request;
    }

    private GetCisesRequest MapPaginationRequest(Pagination pagination)
    {
        var request = new GetCisesRequest
        {
            Filter = new CisesFilter()
            {
            },
            Pagination = pagination
        };

        return request;
    }
}