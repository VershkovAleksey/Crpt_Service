using Application.Extensions;
using DAL;
using Database.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Json;

namespace Application;

public class Startup
{
    public Startup()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.development.json");

        Configuration = configBuilder.Build();
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSettings(Configuration);
        services.AddServices(Configuration);
        services.AddRepositories();
        
        services.AddSwaggerGen(c => { //<-- NOTE 'Add' instead of 'Configure'
            c.SwaggerDoc("v1", new OpenApiInfo {
                Title = "CrptApi",
                Version = "v1"
            });
        });
        
        services.AddDbContext<CrptContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            //app.UseHsts();
        }
        
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseHttpsRedirection();
        app.UseStaticFiles();
    }
}