using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Application.Features.Bot.Strategies;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Application.Features.Bot.Factories;

/// <summary>
/// Implementação da fábrica de estratégias de resposta do bot.
/// Seleciona a estratégia baseada na capacidade de cada uma lidar com o comando.
/// </summary>
public class BotResponseStrategyFactory : IBotResponseStrategyFactory
{
    private readonly IEnumerable<IBotResponseStrategy> _strategies;

    public BotResponseStrategyFactory(IEnumerable<IBotResponseStrategy> strategies)
    {
        // O IEnumerable injetará todas as implementações registradas de IBotResponseStrategy
        _strategies = strategies;
    }

    public IBotResponseStrategy GetStrategy(ProcessUserMessageCommand command)
    {
        // Prioridade: ExitCommand > KeywordBased > Random (fallback)
        // A ordem de registro no DI pode influenciar, mas para maior controle,
        // pode-se ter uma lista ordenada ou lógica de prioridade aqui.
        // Por simplicidade, FirstOrDefault que pode lidar. RandomStrategy sempre pode lidar,
        // então deve ser a última na lista (ou ser explicitamente tratada como fallback).

        // Exemplo de lógica de prioridade:
        if (_strategies.Any(s => s is ExitCommandStrategy && s.CanHandle(command)))
        {
            return _strategies.First(s => s is ExitCommandStrategy);
        }
        if (_strategies.Any(s => s is KeywordBasedResponseStrategy && s.CanHandle(command)))
        {
            return _strategies.First(s => s is KeywordBasedResponseStrategy);
        }
        // Fallback para a estratégia aleatória se nenhuma outra se aplicar
        return _strategies.First(s => s is RandomResponseStrategy);

        // Alternativamente, se CanHandle retornar true apenas para uma estratégia,
        // e RandomResponseStrategy sempre retornar true, a linha abaixo funcionaria
        // desde que RandomResponseStrategy seja a última verificada (ex: se as outras falharem o CanHandle)
        // return _strategies.FirstOrDefault(s => s.CanHandle(command))
        //        ?? throw new InvalidOperationException("No suitable bot response strategy found.");
    }
}