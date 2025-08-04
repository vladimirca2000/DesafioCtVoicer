using ChatBot.Domain.ValueObjects; // Necessário para MessageContent
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Domain.Repositories; // Necessário para IBotResponseRepository
using ChatBot.Domain.Enums; // Necessário para BotResponseType
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratégia de resposta aleatória do bot, geralmente usada como fallback.
/// </summary>
public class RandomResponseStrategy : IBotResponseStrategy
{
    private readonly IBotResponseRepository _botResponseRepository;
    private readonly Random _random = new Random();

    public RandomResponseStrategy(IBotResponseRepository botResponseRepository)
    {
        _botResponseRepository = botResponseRepository;
    }

    public bool CanHandle(ProcessUserMessageCommand command)
    {
        // Esta estratégia pode sempre lidar, servindo como um fallback se nenhuma outra se aplicar.
        return true;
    }

    public MessageContent GenerateResponse(ProcessUserMessageCommand command)
    {
        var randomResponses = _botResponseRepository
            .GetAllAsync()
            .Result // ATENÇÃO: Evite .Result em código assíncrono real.
            .Where(r => r.Type == BotResponseType.Random)
            .ToList();

        if (randomResponses.Any())
        {
            var randomIndex = _random.Next(randomResponses.Count);
            return MessageContent.Create(randomResponses[randomIndex].Content);
        }
        return MessageContent.Create("Olá! Como posso ajudar?"); // Resposta padrão se não houver respostas aleatórias configuradas
    }
}