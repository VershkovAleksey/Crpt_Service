using System.Collections;
using Domain.Models.Crpt.Marking.Dto;
using Domain.Models.Crpt.Marking.Request;

namespace Abstractions.Services;

public interface IMarkingService
{
    public Task<List<MarkingListDto>> GetIdentificationCodesAsync(string token, IEnumerable<string> gtins,
        CancellationToken cancellationToken = default);

    public Task<CreateDocumentBodyRequest> CreateSetsAsync(string token, int userId,
        CancellationToken cancellationToken = default);

    public Task SignDataAsync(string token, int userId, CreateDocumentBodyRequest createDocumentBodyRequest,
        CancellationToken cancellationToken = default);
}