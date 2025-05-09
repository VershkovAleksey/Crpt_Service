using Abstractions.Infrastructure.Settings;

namespace Domain.Settings;

public sealed class CrptHttpSettings : HttpClientSettingsBase
{
    
    public static string SettingName { get; set; } = nameof(CrptHttpSettings);
    
    public required string UrlV4 { get; set; }
}