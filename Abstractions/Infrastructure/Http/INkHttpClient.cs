using Domain.Models.NationalCatalog.Responses;

namespace Abstractions.Infrastructure.Http;

public interface INkHttpClient
{
    public Task<GetProductListResponse?> GetProductListAsync(CancellationToken cancellationToken = default);
    
    public Task<ProductDetailInfoResponse?> GetProductDetailInfoAsync(string gtin , CancellationToken cancellationToken = default);
}