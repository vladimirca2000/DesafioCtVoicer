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
        
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();

        
        services.AddDbContext<ChatBotDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            
            var interceptors = serviceProvider.GetServices<ISaveChangesInterceptor>();
            options.AddInterceptors(interceptors);
        });

       
        services.AddScoped<IUnitOfWork, UnitOfWork>();

       
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        
        services.AddScoped<BotResponseRepository>(); 

        
        services.AddScoped<IBotResponseRepository, CachedBotResponseRepository>(provider =>
        {
            var concreteRepo = provider.GetRequiredService<BotResponseRepository>(); 
            var cacheService = provider.GetRequiredService<ICacheService>();     
            return new CachedBotResponseRepository(concreteRepo, cacheService);   
        });

        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICacheService, CacheService>();

        
        services.AddDistributedMemoryCache(); // Usado para desenvolvimento/teste.
        

        return services;
    }
}