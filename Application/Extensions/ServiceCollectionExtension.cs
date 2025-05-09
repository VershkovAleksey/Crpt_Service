using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using BL.Infrastructure.Http;
using BL.Services;

namespace Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ICrptHttpClient, CrptHttpClient>();
        services.AddScoped<INkHttpClient, NkHttpClient>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMarkingService, MarkingService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<INationalCatalogService, NationalCatalogService>();
        return services;
    }
}