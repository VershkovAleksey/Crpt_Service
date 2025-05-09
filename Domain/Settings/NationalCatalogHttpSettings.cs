using Abstractions.Infrastructure.Settings;

namespace Domain.Settings;

public class NationalCatalogHttpSettings : HttpClientSettingsBase
{
    public static string SettingName { get; set; } = nameof(NationalCatalogHttpSettings);
}