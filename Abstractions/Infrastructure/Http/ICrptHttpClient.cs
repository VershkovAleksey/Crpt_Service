using Domain.Models.Crpt.Auth;
using Domain.Models.Crpt.Marking.Request;
using Domain.Models.Crpt.Marking.Response;

namespace Abstractions.Infrastructure.Http;

/// <summary>
/// Клиент запросов в CRPT
/// </summary>
public interface ICrptHttpClient
{
    /// <summary>
    /// Получение данных для подписания
    /// </summary>
    /// <param name="cancellationToken">Токен завершения</param>
    /// <returns>Объект с данными <see cref="AuthResponseDataDto"/></returns>
    public Task<AuthResponseDataDto?> GetAuthDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение токена
    /// </summary>
    /// <param name="request">Тело запроса с подписанными УКЭП данными <see cref="AuthSignedRequest"/></param>
    /// <param name="cancellationToken">Токен завершения</param>
    /// <returns>Объект с результатом запроса <see cref="AuthSignInResponseDto"/></returns>
    public Task<AuthSignInResponseDto?> GetTokenAsync(AuthSignedRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение кодов идентификации
    /// </summary>
    /// <param name="token">Токен авторизации</param>
    /// <param name="request">Тело запроса</param>
    /// <param name="cancellationToken">Токен завершения</param>
    public Task<GetCisesResponse?> GetCisesAsync(string token, GetCisesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Формирование наборов
    /// </summary>
    /// <param name="token">Токен авторизации</param>
    /// <param name="request">Тело запроса</param>
    /// <param name="cancellationToken">Токен завершения</param>
    Task<CreateSetsResponse?> CreateSetsAsync(string token, CreateDocumentBodyRequest request,
        CancellationToken cancellationToken = default);
}