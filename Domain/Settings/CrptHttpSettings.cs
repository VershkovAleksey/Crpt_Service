using Abstractions.Infrastructure.Settings;

namespace Domain.Settings;

public class CrptHttpSettings : HttpClientSettingsBase
{
    
    public static string SettingName { get; set; } = nameof(CrptHttpSettings);
    
    public override required string Url { get; set; }
    
    public required string UrlV4 { get; set; }
    public override required string ClientName { get; set; }
}