using Abstractions.Repositories;
using Database.Context;
using Database.Entities.Sets;
using Domain.Models.Entities;

namespace DAL.Repositories;

public class SetsRepository(CrptContext context) : RepositoryBase(context), ISetsRepository
{
    private readonly CrptContext _crptContext = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<int> AddSetAsync(SetDto set, CancellationToken cancellationToken = default)
    {
        return await AddAsync(set);
    }

    public async Task<bool> AddRangeAsync(IEnumerable<SetDto>? sets, CancellationToken cancellationToken = default)
    {
        if (sets is null)
        {
            return false;
        }

        var setDtos = sets.ToList();

        if (setDtos.Count == 0)
            return false;

        var list = setDtos.Select(MapSet).ToList();

        await _crptContext.AddRangeAsync(list, cancellationToken);
        return true;
    }

    private SetEntity MapSet(SetDto setDto)
    {
        return new SetEntity()
        {
            SetName = setDto.SetName,
            Gtin = setDto.Gtin
        };
    }
}