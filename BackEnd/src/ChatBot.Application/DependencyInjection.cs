using ChatBot.Application.Common.Behaviors;
using ChatBot.Application.Features.Bot.Factories;
using ChatBot.Application.Features.Bot.Strategies;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

        // Registro das Estratégias de Resposta do Bot
        // Devem ser registradas como Transient para que uma nova instância seja criada por requisição,
        // garantindo que elas não mantenham estado entre diferentes chamadas.
        services.AddTransient<IBotResponseStrategy, ExitCommandStrategy>();
        services.AddTransient<IBotResponseStrategy, RandomResponseStrategy>();
        services.AddTransient<IBotResponseStrategy, KeywordBasedResponseStrategy>();

        // Registro da Fábrica de Estratégias do Bot
        services.AddScoped<IBotResponseStrategyFactory, BotResponseStrategyFactory>();

        return services;
    }
}