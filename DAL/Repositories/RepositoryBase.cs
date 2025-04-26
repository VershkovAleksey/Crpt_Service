using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Context;

namespace DAL.Repositories;

public class RepositoryBase(CrptContext context) : IDisposable, IAsyncDisposable
{
    private readonly CrptContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<int> AddAsync<T>(T entity)
    {
        if (entity is null)
            return 0;

        await _context.AddAsync(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync<T>(T entity)
    {
        if (entity is null)
        {
            return 0;
        }

        _context.Update(entity);

        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateRange<T>(IEnumerable<T> entities)
    {
        if (!entities.Any())
            return 0;

        _context.UpdateRange(entities);

        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}