using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Repositories;
using ChatBot.Infrastructure.Data;
using ChatBot.Infrastructure.Data.Interceptors;
using ChatBot.Infrastructure.Repositories;
using ChatBot.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Registro dos interceptors (como scoped)
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

        // Database
        services.AddDbContext<ChatBotDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            // Adicionar interceptors via serviceProvider
            var interceptors = serviceProvider.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(interceptors);
        });

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositórios
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        // Registro do repositório concreto de BotResponse
        services.AddScoped<BotResponseRepository>(); // Implementação concreta

        // Registro de IBotResponseRepository com o decorator de cache
        services.AddScoped<IBotResponseRepository, CachedBotResponseRepository>(provider =>
        {
            var concreteRepo = provider.GetRequiredService<BotResponseRepository>(); // Obtém a implementação concreta
            var cacheService = provider.GetRequiredService<ICacheService>();      // Obtém o serviço de cache
            return new CachedBotResponseRepository(concreteRepo, cacheService);   // Retorna o decorator
        });

        // Serviços complementares
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICacheService, CacheService>();

        // Configuração do cache distribuído (MemoryCache para desenvolvimento, Redis/SQL Server para produção)
        services.AddDistributedMemoryCache(); // Usado para desenvolvimento/teste.
        // Para produção, configure um cache distribuído real (Redis, SQL Server):
        // services.AddStackExchangeRedisCache(options => { options.Configuration = configuration.GetConnectionString("Redis"); });
        // services.AddDistributedSqlServerCache(options => { options.ConnectionString = configuration.GetConnectionString("SqlServerCache"); });

        return services;
    }
}