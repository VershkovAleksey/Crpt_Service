using Abstractions.Repositories;
using DAL.Repositories;
using Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<CrptContext>();
        
        services.AddScoped<ISetsRepository, SetsRepository>();
        services.AddScoped<IUnitsRepository, UnitsRepository>();
        return services;
    }
}