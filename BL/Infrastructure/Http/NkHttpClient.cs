using Abstractions.Infrastructure.Http;
using Domain.Models.NationalCatalog.Responses;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BL.Infrastructure.Http;

public sealed class NkHttpClient(
    IHttpClientFactory httpClientFactory,
    IOptions<NationalCatalogHttpSettings> settings,
    ILogger<NkHttpClient> logger)
    : HttpClientBase(httpClientFactory, settings), INkHttpClient
{
    private readonly ILogger<NkHttpClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    const string apiKey = "2607ohqne9btaq8w";

    private readonly IOptions<NationalCatalogHttpSettings> _settings =
        settings ?? throw new ArgumentNullException(nameof(settings));

    public async Task<GetProductListResponse?> GetProductListAsync(CancellationToken cancellationToken = default)
    {
        var fromDate = DateTime.Now.AddDays(-20).ToString("yyyy-MM-dd HH:mm:ss");
        var routePostfix = $"/v4/product-list?apikey={apiKey}&from_date={fromDate}";
        return await GetAsync<GetProductListResponse>(_settings.Value.Url + routePostfix, cancellationToken);
    }

    public async Task<ProductDetailInfoResponse?> GetProductDetailInfoAsync(string gtin, CancellationToken cancellationToken = default)
    {
        var routePostfix = $"/v3/feed-product?apikey={apiKey}&gtin={gtin}";
        return await GetAsync<ProductDetailInfoResponse>(_settings.Value.Url + routePostfix, cancellationToken);
    }
}