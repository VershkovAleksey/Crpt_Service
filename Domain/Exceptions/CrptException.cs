namespace Domain.Exceptions;

/// <summary>
/// Ошибка при запросе в CRPT
/// </summary>
public class CrptException : Exception
{
    /// <summary>
    /// Статус код
    /// </summary>
    public string? StatusCode { get; }
    
    /// <summary>
    /// Тело ответа
    /// </summary>
    public string? HttpResponseContent { get; }
    
    /// <summary>
    /// Внутренняя ошибка
    /// </summary>
    public new Exception? InnerException { get; }

    public CrptException(string statusCode, string httpResponseContent, string message, Exception? innerException) : base(message)
    {
        StatusCode = statusCode;
        HttpResponseContent = httpResponseContent;
        InnerException = innerException;
    }

    public CrptException(string message, Exception? innerException) : base(message)
    {
        InnerException = innerException;
    }
}