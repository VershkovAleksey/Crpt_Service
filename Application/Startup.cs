using Application.Extensions;
using Application.Extensions.Middlewares;
using BL.Services;
using DAL;
using Database.Context;
using Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Json;

namespace Application;

public class Startup
{
    public Startup(IWebHostEnvironment env)
    {
        var configBuilder = new ConfigurationBuilder();

        if (env.IsDevelopment())
        {
            configBuilder.AddJsonFile("appsettings.development.json");
        }

        if (env.IsProduction())
        {
            configBuilder.AddJsonFile("appsettings.json");
        }

        Configuration = configBuilder.Build();
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddControllers();


        services.AddDbContext<CrptContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        services.AddCors(options =>
        {
            options.AddPolicy("VuePolicy",
                builder => builder.WithOrigins("http://130.193.52.139:8081", "http://130.193.52.139:8080", "http://130.193.52.139:80")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // укзывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представляющая издателя
                    ValidIssuer = AuthOptions.ISSUER,

                    // будет ли валидироваться потребитель токена
                    ValidateAudience = false,
                    // установка потребителя токена
                    ValidAudience = AuthOptions.AUDIENCE,
                    // будет ли валидироваться время существования
                    ValidateLifetime = true,

                    // установка ключа безопасности
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                };
            });

        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddSettings(Configuration);
        services.AddServices(Configuration);

        //services.AddTransient<CurrentUserMiddleware>();

        services.AddSwaggerGen(c =>
        {
            //<-- NOTE 'Add' instead of 'Configure'
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CrptApi",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddRepositories();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            // app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
            // {
            //     options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            // });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            //app.UseHsts();
        }

        app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });
        
        app.UseRouting();
        app.UseCors("VuePolicy");

        app.UseCurrentUserMiddleware();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        //app.UseHttpsRedirection();
        app.UseStaticFiles();
        
    }
}