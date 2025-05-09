using Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class Config
{
    public static IServiceCollection AddSettings(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions();
        services.Configure<CrptHttpSettings>(config.GetSection(nameof(CrptHttpSettings)));
        services.Configure<NationalCatalogHttpSettings>(config.GetSection(nameof(NationalCatalogHttpSettings)));
        return services;
    }
}