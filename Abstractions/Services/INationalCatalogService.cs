using Domain.Models.NationalCatalog.Dto;
using Domain.Models.NationalCatalog.Responses;

namespace Abstractions.Services;

public interface INationalCatalogService
{
    Task<GetProductListResponse?> GetProductListAsync(CancellationToken cancellationToken = default);

    public Task SeedDataAsync(CancellationToken cancellationToken = default);

    Task<List<SetOptionDto>> GetSetsAsync();
    
    Task<bool> CreateSetsAsync(IEnumerable<SetOptionDto> options, CancellationToken cancellationToken = default);

    Task<List<CreatedSetsDto>> GetSetsByUserIdAsync(int userId);

    Task DeleteItemAsync(int itemId, CancellationToken cancellationToken = default);
}