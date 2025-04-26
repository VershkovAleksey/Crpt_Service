using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models.Crpt.Marking.Dto;

namespace Abstractions.Services;

public interface IMarkingService
{
    public Task<List<MarkingListDto>?> GetIdentificationCodesAsync(CancellationToken cancellationToken = default);
    Task CreateSetsAsync();
}