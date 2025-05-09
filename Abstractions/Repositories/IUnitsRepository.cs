using Domain.Models.Entities;

namespace Abstractions.Repositories;

public interface IUnitsRepository
{
    Task<int> AddSetAsync(UnitDto unit, CancellationToken cancellationToken = default);
    
    Task<bool> AddRangeAsync(IEnumerable<UnitDto>? units, CancellationToken cancellationToken = default);
}