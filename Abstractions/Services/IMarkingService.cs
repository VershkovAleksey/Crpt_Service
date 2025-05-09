using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models.Crpt.Marking.Dto;
using Domain.Models.Crpt.Marking.Request;
using Domain.Models.NationalCatalog.Dto;

namespace Abstractions.Services;

public interface IMarkingService
{
    public Task<List<MarkingListDto>?> GetIdentificationCodesAsync(string token, CancellationToken cancellationToken = default);

    public Task<CreateDocumentBodyRequest> CreateSetsAsync(string token, int userId,
        CancellationToken cancellationToken = default);

    public Task SignDataAsync(string token, int userId, CreateDocumentBodyRequest createDocumentBodyRequest,
        CancellationToken cancellationToken = default);
}