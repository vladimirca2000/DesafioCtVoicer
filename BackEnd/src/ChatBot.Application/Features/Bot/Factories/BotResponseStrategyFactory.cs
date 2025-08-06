using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Application.Features.Bot.Strategies;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Features.Bot.Factories;

/// <summary>
/// Implementação da fábrica de estratégias de resposta do bot.
/// Seleciona a estratégia baseada na capacidade de cada uma lidar com o comando.
/// </summary>
public class BotResponseStrategyFactory : IBotResponseStrategyFactory
{
    private readonly IEnumerable<IBotResponseStrategy> _strategies;
    private readonly ILogger<BotResponseStrategyFactory> _logger;

    public BotResponseStrategyFactory(IEnumerable<IBotResponseStrategy> strategies, ILogger<BotResponseStrategyFactory> logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    public async Task<IBotResponseStrategy> GetStrategy(ProcessUserMessageCommand command)
    {
        _logger.LogInformation("Selecionando estratégia para mensagem: '{Message}'", command.UserMessage);

        // 1. Verificar comando de saída primeiro
        var exitStrategy = _strategies.FirstOrDefault(s => s is ExitCommandStrategy);
        if (exitStrategy != null && await exitStrategy.CanHandle(command))
        {
            _logger.LogInformation("Estratégia selecionada: ExitCommandStrategy");
            return exitStrategy;
        }

        // 2. Verificar estratégia baseada em palavras-chave
        var keywordStrategy = _strategies.FirstOrDefault(s => s is KeywordBasedResponseStrategy);
        if (keywordStrategy != null && await keywordStrategy.CanHandle(command))
        {
            _logger.LogInformation("Estratégia selecionada: KeywordBasedResponseStrategy");
            return keywordStrategy;
        }

        // 3. Fallback para estratégia aleatória
        var randomStrategy = _strategies.FirstOrDefault(s => s is RandomResponseStrategy);
        if (randomStrategy != null)
        {
            _logger.LogInformation("Estratégia selecionada: RandomResponseStrategy (fallback)");
            return randomStrategy;
        }

        _logger.LogError("Nenhuma estratégia de resposta do bot foi encontrada!");
        throw new InvalidOperationException("Nenhuma estratégia de resposta do bot foi configurada.");
    }
}