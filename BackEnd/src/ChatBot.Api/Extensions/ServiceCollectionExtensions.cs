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

        // SignalR
        services.AddSignalR();

        // CORS
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

        // Registro de IHttpContextAccessor e ICurrentUserService
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Registro do serviço SignalR, que está na camada de API
        services.AddScoped<ISignalRChatService, SignalRChatService>();

        return services;
    }
}