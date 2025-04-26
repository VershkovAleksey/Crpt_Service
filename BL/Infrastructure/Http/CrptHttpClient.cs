using Abstractions.Infrastructure.Http;
using Domain.Models.Crpt.Auth;
using Domain.Models.Crpt.Marking.Request;
using Domain.Models.Crpt.Marking.Response;
using Domain.Settings;
using Microsoft.Extensions.Options;

namespace BL.Infrastructure.Http;

/// <summary>
/// Клиент запросов в CRPT
/// </summary>
public sealed class CrptHttpClient(
    IHttpClientFactory httpClientFactory,
    IOptions<CrptHttpSettings> settings)
    : HttpClientBase(httpClientFactory, settings), ICrptHttpClient
{
    private readonly IOptions<CrptHttpSettings> _settings =
        settings ?? throw new ArgumentNullException(nameof(settings));

    public async Task<AuthResponseDataDto?> GetAuthDataAsync(CancellationToken cancellationToken = default)
    {
        const string routePostfix = "auth/key";
        var headers = new Dictionary<string, string> { { "accept", "application/json" } };
        return await GetAsync<AuthResponseDataDto>(_settings.Value.Url + routePostfix, cancellationToken, headers);
    }

    public async Task<AuthSignInResponseDto?> GetTokenAsync(AuthSignedRequest request,
        CancellationToken cancellationToken = default)
    {
        const string routePostfix = "auth/simpleSignIn";
        var headers = new Dictionary<string, string>
            { { "accept", "application/json" } };

        return await SendRequestAsync<AuthSignedRequest, AuthSignInResponseDto>(string.Empty,
            headers, routePostfix, false, "post", request,
            cancellationToken);
    }

    public async Task<GetCisesResponse?> GetCisesAsync(string token, GetCisesRequest request,
        CancellationToken cancellationToken = default)
    {
        const string routePostfix = "cises/search";

        return await SendRequestAsync<GetCisesRequest, GetCisesResponse>(token,
            new Dictionary<string, string>(), routePostfix, true, "post", request,
            cancellationToken);
    }

    public async Task<CreateSetsResponse?> CreateSetsAsync(string token, CreateDocumentBodyRequest request,
        CancellationToken cancellationToken = default)
    {
        const string routePostfix = "lk/documents/create?pg=lp";

        return await SendRequestAsync<CreateDocumentBodyRequest, CreateSetsResponse>(token,
            new Dictionary<string, string>(), routePostfix, true, "post", request,
            cancellationToken);
    }

    private async Task<TResponse?> SendRequestAsync<TRequest, TResponse>(
        string token, Dictionary<string, string> headers, string routePostfix, bool isApiV4, string requestMethod,
        TRequest request, CancellationToken cancellationToken = default, bool serialized = false)
        where TRequest : class where TResponse : class
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            headers.Add("Authorization", $"Bearer {token}");
        }

        var url = $"{(isApiV4 ? _settings.Value.UrlV4 : _settings.Value.Url)}{routePostfix}";

        return requestMethod.ToUpper() switch
        {
            ("GET") => await GetAsync<TResponse>(url, cancellationToken, headers),
            ("POST") => serialized
                ? await PostSerialaizedAsync<TRequest, TResponse>(url, request, cancellationToken, headers)
                : await PostAsync<TRequest, TResponse>(url, request, cancellationToken, headers),
            _ => null
        };
    }
}