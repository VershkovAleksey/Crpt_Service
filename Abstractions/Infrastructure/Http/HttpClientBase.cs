using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Extensions;
using Abstractions.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Abstractions.Infrastructure.Http;

/// <summary>
/// Базовая реализация Http клиента
/// </summary>
public class HttpClientBase
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Конструктор класса <see cref="HttpClientBase"/>
    /// </summary>
    /// <param name="httpClientFactory">Фабрика Http клиентов</param>
    /// <param name="settings">Настройки клиента</param>
    protected HttpClientBase(
        IHttpClientFactory httpClientFactory,
        IOptions<HttpClientSettingsBase> settings)
    {
        _httpClient = httpClientFactory.CreateClient(settings.Value.ClientName);
    }

    /// <summary>
    /// Отправка Get запроса
    /// </summary>
    /// <param name="url">Адрес запроса</param>
    /// <param name="cancellationToken">Токен завершения</param>
    /// <param name="headers">Заголовки запроса</param>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <exception cref="Exception">Ошибка при отправке запроса</exception>
    protected async Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken cancellationToken,
        IDictionary<string, string>? headers = null)
    {
        try
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await SendAsync<TResponse>(httpRequestMessage, cancellationToken, headers);

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while getting from {url}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Отправка Post запроса
    /// </summary>
    /// <param name="url">Адрес запроса</param>
    /// <param name="request">Сущность сообщения</param>
    /// <param name="cancellationToken">Токен завершения</param>
    /// <param name="headers">Заголовки запроса</param>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <exception cref="Exception">Ошибка при отправке запроса</exception>
    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request,
        CancellationToken cancellationToken,
        IDictionary<string, string>? headers = null)
    {
        try
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            httpRequestMessage.Content = JsonContent.Create(request);

            return await SendAsync<TResponse>(httpRequestMessage, cancellationToken, headers);
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while posting to {url}: {ex.Message}", ex);
        }
    }

    protected async Task<TResponse?> PostSerialaizedAsync<TRequest, TResponse>(string url, TRequest request,
        CancellationToken cancellationToken, IDictionary<string, string>? headers = null)
    {
        try
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(request));

            headers ??= new Dictionary<string, string>();

            headers.Add("Content-Type", "application/json");

            return await SendAsync<TResponse>(httpRequestMessage, cancellationToken, headers);
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while posting to {url}: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// Отправка запроса с указанным типом
    /// </summary>
    /// <param name="request">Сущность сообщения</param>
    /// <param name="cancellationToken">Токен завершения</param>
    /// <param name="headers">Заголовки запроса</param>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    private async Task<TResponse?> SendAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken,
        IDictionary<string, string>? headers = null)
    {
        if (headers != null)
        {
            request.SetHeaders(headers);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonConvert.DeserializeObject<TResponse>(responseString);
    }
}