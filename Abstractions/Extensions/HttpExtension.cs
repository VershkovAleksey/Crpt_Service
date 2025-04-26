namespace Abstractions.Extensions;

/// <summary>
/// Статичный класс расширений для Http клиентов
/// </summary>
public static class HttpExtension
{
    /// <summary>
    /// Установить заголовки запроса
    /// </summary>
    /// <param name="request">Объект запроса</param>
    /// <param name="headers">Словарь заголовков</param>
    public static HttpRequestMessage SetHeaders(this HttpRequestMessage request, IDictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        return request;
    }
}