using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using Domain.Models.NationalCatalog.Responses;
using Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BL.Infrastructure.Http;

public sealed class NkHttpClient(
    IHttpClientFactory httpClientFactory,
    IOptions<NationalCatalogHttpSettings> settings,
    ILogger<NkHttpClient> logger,
    ICurrentUserService currentUserService)
    : HttpClientBase(httpClientFactory, settings), INkHttpClient
{
    private readonly ILogger<NkHttpClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly ICurrentUserService _currentUserService =
        currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

    private readonly IOptions<NationalCatalogHttpSettings> _settings =
        settings ?? throw new ArgumentNullException(nameof(settings));

    public async Task<GetProductListResponse?> GetProductListAsync(CancellationToken cancellationToken = default)
    {
        var fromDate = DateTime.Now.AddYears(-1).AddMonths(-6).ToString("yyyy-MM-dd HH:mm:ss");
        var toDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var routePostfix =
            $"/v4/product-list?apikey={_currentUserService.CurrentUser.NkApiKey}&from_date={fromDate}&to_date={toDate}";
        return await GetAsync<GetProductListResponse>(_settings.Value.Url + routePostfix, cancellationToken);
    }

    public async Task<ProductDetailInfoResponse?> GetProductDetailInfoAsync(string gtin,
        CancellationToken cancellationToken = default)
    {
        var routePostfix = $"/v3/feed-product?apikey={_currentUserService.CurrentUser.NkApiKey}&gtin={gtin}";
        return await GetAsync<ProductDetailInfoResponse>(_settings.Value.Url + routePostfix, cancellationToken);
    }
}