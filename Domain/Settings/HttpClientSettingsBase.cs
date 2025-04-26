namespace Abstractions.Infrastructure.Settings;

/// <summary>
/// Базовый класс настроек Http клиентов
/// </summary>
public abstract class HttpClientSettingsBase
{
    /// <summary>
    /// Адрес
    /// </summary>
    public abstract required string Url { get; set; }
    
    /// <summary>
    /// Имя клиента
    /// </summary>
    public abstract required string ClientName { get; set; }
}