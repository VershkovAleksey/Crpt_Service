using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using BL.Infrastructure.Http;
using BL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ICrptHttpClient, CrptHttpClient>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMarkingService, MarkingService>();
        return services;
    }
}