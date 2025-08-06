// Conteúdo COMPLETO para ChatBot.Application/Features/Bot/Strategies/RandomResponseStrategy.cs

using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks; // Adicionar este using
using System; // Necessário para Random
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratégia de resposta aleatória do bot, geralmente usada como fallback.
/// </summary>
public class RandomResponseStrategy : IBotResponseStrategy
{
    private readonly IBotResponseRepository _botResponseRepository;
    private readonly ILogger<RandomResponseStrategy> _logger;
    private readonly Random _random = new Random();

    public RandomResponseStrategy(IBotResponseRepository botResponseRepository, ILogger<RandomResponseStrategy> logger)
    {
        _botResponseRepository = botResponseRepository;
        _logger = logger;
    }

    public async Task<bool> CanHandle(ProcessUserMessageCommand command)
    {
        // Esta estratégia pode sempre lidar, servindo como um fallback se nenhuma outra se aplicar.
        return await Task.FromResult(true);
    }

    // ALTERADO: Agora é assíncrono e usa await para GetAllAsync()
    public async Task<MessageContent> GenerateResponse(ProcessUserMessageCommand command)
    {
        _logger.LogInformation("Buscando respostas aleatórias no banco de dados...");
        
        var allResponses = await _botResponseRepository.GetAllAsync();
        _logger.LogInformation("Total de respostas encontradas: {Count}", allResponses.Count());
        
        var randomResponses = allResponses
            .Where(r => r.Type == BotResponseType.Random && r.IsActive && !r.IsDeleted)
            .ToList();

        _logger.LogInformation("Respostas aleatórias disponíveis: {Count}", randomResponses.Count);

        if (randomResponses.Any())
        {
            var randomIndex = _random.Next(randomResponses.Count);
            var selectedResponse = randomResponses[randomIndex];
            _logger.LogInformation("Resposta selecionada: '{Content}'", selectedResponse.Content);
            return MessageContent.Create(selectedResponse.Content);
        }
        
        _logger.LogWarning("Nenhuma resposta aleatória encontrada no banco. Usando resposta padrão.");
        return MessageContent.Create("Olá! Como posso ajudar?"); // Resposta padrão se não houver respostas aleatórias configuradas
    }
}