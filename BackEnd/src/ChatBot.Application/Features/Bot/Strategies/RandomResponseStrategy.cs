// Conteúdo COMPLETO para ChatBot.Application/Features/Bot/Strategies/RandomResponseStrategy.cs

using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks; // Adicionar este using
using System; // Necessário para Random

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratgia de resposta aleatria do bot, geralmente usada como fallback.
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
        // Esta estratgia pode sempre lidar, servindo como um fallback se nenhuma outra se aplicar.
        return true;
    }

    // ALTERADO: Agora é assíncrono e usa await para GetAllAsync()
    public async Task<MessageContent> GenerateResponse(ProcessUserMessageCommand command)
    {
        var randomResponses = (await _botResponseRepository.GetAllAsync()) // ALTERADO: Removido .Result, adicionado await
            .Where(r => r.Type == BotResponseType.Random)
            .ToList();

        if (randomResponses.Any())
        {
            var randomIndex = _random.Next(randomResponses.Count);
            return MessageContent.Create(randomResponses[randomIndex].Content);
        }
        return MessageContent.Create("Ol! Como posso ajudar?"); // Resposta padro se no houver respostas aleatrias configuradas
    }
}