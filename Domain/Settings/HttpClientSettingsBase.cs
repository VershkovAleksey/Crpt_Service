namespace Abstractions.Infrastructure.Settings;

/// <summary>
/// Базовый класс настроек Http клиентов
/// </summary>
public class HttpClientSettingsBase
{
    /// <summary>
    /// Адрес
    /// </summary>
    public required string Url { get; set; }
    
    /// <summary>
    /// Имя клиента
    /// </summary>
    public required string ClientName { get; set; }
}