using Abstractions.Repositories;
using Database.Context;
using Domain.Models.Entities;

namespace DAL.Repositories;

public class UnitsRepository(CrptContext context) : RepositoryBase(context), IUnitsRepository
{
    private readonly CrptContext _crptContext = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<int> AddSetAsync(UnitDto set, CancellationToken cancellationToken = default)
    {
        return await AddAsync(set);
    }

    public async Task<bool> AddRangeAsync(IEnumerable<UnitDto>? units, CancellationToken cancellationToken = default)
    {
        if (units is null)
        {
            return false;
        }

        var unitDtos = units.ToList();

        if (unitDtos.Count == 0)
            return false;

        await _crptContext.AddRangeAsync(unitDtos, cancellationToken);
        return true;
    }
}