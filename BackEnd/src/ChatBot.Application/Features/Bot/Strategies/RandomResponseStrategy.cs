using ChatBot.Domain.Enums;
using ChatBot.Domain.Repositories;
using Microsoft.Extensions.Logging; // Opcional, para logar a seleção da resposta

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratégia que seleciona uma resposta aleatória do bot.
/// </summary>
public class RandomResponseStrategy : IBotResponseStrategy
{
    private readonly IBotResponseRepository _botResponseRepository;
    private readonly ILogger<RandomResponseStrategy> _logger; // Para fins de depuração

    public RandomResponseStrategy(IBotResponseRepository botResponseRepository,
                                  ILogger<RandomResponseStrategy> logger)
    {
        _botResponseRepository = botResponseRepository;
        _logger = logger;
    }

    public async Task<string?> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken)
    {
        // Obter todas as respostas de tipo 'Random'
        var randomResponses = (await _botResponseRepository
            .GetAllAsync(cancellationToken))
            .Where(r => r.Type == BotResponseType.Random && r.IsActive)
            .ToList();

        if (!randomResponses.Any())
        {
            _logger.LogWarning("Nenhuma resposta aleatória ativa encontrada no repositório.");
            return null; // Nenhuma resposta aleatória disponível
        }

        // Selecionar uma resposta aleatória da lista
        var randomIndex = new Random().Next(randomResponses.Count);
        var selectedResponse = randomResponses[randomIndex];

        _logger.LogInformation("Resposta aleatória selecionada: {Content}", selectedResponse.Content);
        return selectedResponse.Content;
    }
}
