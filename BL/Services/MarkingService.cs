using System.Text;
using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using Domain.Models.Crpt.Marking.Dto;
using Domain.Models.Crpt.Marking.Request;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BL.Services;

public sealed class MarkingService(
    ILogger<MarkingService> logger,
    ICrptHttpClient crptHttpClient,
    IAuthService authService) : IMarkingService
{
    private readonly ILogger<MarkingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly ICrptHttpClient _crptHttpClient =
        crptHttpClient ?? throw new ArgumentNullException(nameof(crptHttpClient));

    private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));

    public async Task<List<MarkingListDto>?> GetIdentificationCodesAsync(CancellationToken cancellationToken = default)
    {
        var token = await _authService.GetTokenAsync();

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
            await _crptHttpClient.GetCisesAsync(token, MapPaginationRequest(pagination), cancellationToken);

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

    public async Task CreateSetsAsync()
    {
        var token = await _authService.GetTokenAsync();

        var cises = await GetIdentificationCodesAsync();

        var sets = cises.Where(x => x.CisesType == "SET").ToList();

        var units = cises.Where(x => x.CisesType != "SET").ToList();

        var aggregationUnits = new List<AggregationUnit>();
        var oneSet = new AggregationUnit()
        {
            UnitSerialNumber = sets.First().Cises.First().Cis,
            Sntins = new[]
            {
                units.First().Cises.First().Cis,
                units.Last().Cises.First().Cis
            }
        };

        aggregationUnits.Add(oneSet);

        var requestBody = new CreateSetsRequest()
        {
            AggregationUnits = aggregationUnits,
            ParticipantId = "212702137805"
        };

        var requestBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestBody));

        var request = new CreateDocumentBodyRequest()
        {
            ProductDocument = Convert.ToBase64String(requestBytes),
            Signature = _authService.SignData(requestBytes, "Быченкова", true)
        };

        await _crptHttpClient.CreateSetsAsync(token, request);
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