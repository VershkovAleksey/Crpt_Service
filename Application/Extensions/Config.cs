using Domain.Settings;

namespace Application.Extensions;

public static class Config
{
    public static IServiceCollection AddSettings(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions();
        services.Configure<CrptHttpSettings>(config.GetSection(nameof(CrptHttpSettings)));
        return services;
    }
}