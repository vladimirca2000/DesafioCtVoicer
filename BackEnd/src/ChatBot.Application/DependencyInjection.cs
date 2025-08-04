using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ChatBot.Application.Common.Behaviors;
using ChatBot.Application.Features.Bot.Factories; // Para registrar a fábrica
using ChatBot.Application.Features.Bot.Strategies; // Para registrar as estratégias

namespace ChatBot.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Pipeline Behaviors (ordem importa!)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        // Estratégias de resposta do Bot
        services.AddScoped<IBotResponseStrategy, ExitCommandStrategy>();
        services.AddScoped<IBotResponseStrategy, KeywordBasedResponseStrategy>();
        services.AddScoped<IBotResponseStrategy, RandomResponseStrategy>(); // Deve ser o fallback

        // Fábrica de estratégias do Bot (Scoped para evitar "captive dependency")
        services.AddScoped<IBotResponseStrategyFactory, BotResponseStrategyFactory>();

        return services;
    }
}