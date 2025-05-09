using Domain.Models.Entities;

namespace Abstractions.Repositories;

public interface ISetsRepository
{
    Task<int> AddSetAsync(SetDto set, CancellationToken cancellationToken = default);
    
    Task<bool> AddRangeAsync(IEnumerable<SetDto>? sets, CancellationToken cancellationToken = default);
}