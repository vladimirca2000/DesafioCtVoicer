using ChatBot.Application.Common.Interfaces;
using ChatBot.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ChatBot.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona e configura os serviços específicos da camada de API.
    /// </summary>
    /// <param name="services">A coleção de serviços.</param>
    /// <param name="configuration">A configuração da aplicação.</param>
    /// <returns>A coleção de serviços configurada.</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        
        services.AddSignalR();

        
        services.AddHealthChecks()
            .AddNpgSql(
                connectionString: configuration.GetConnectionString("DefaultConnection")!,
                name: "postgresql",
                tags: new[] { "database", "postgresql" })
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running"), 
                tags: new[] { "api" });

        
        services.AddCors(options =>
        {
            options.AddPolicy("SpecificCorsPolicy", policy =>
            {
                var allowedOrigins = configuration.GetSection("CorsOrigins").Get<string[]>();
                if (allowedOrigins != null && allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                }
                else
                {
                    policy.DisallowCredentials();
                    policy.WithOrigins("null");
                }
            });
        });

        
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        
        services.AddScoped<ISignalRChatService, SignalRChatService>();

        return services;
    }
}