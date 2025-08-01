using ChatBot.Domain.Entities;
using ChatBot.Domain.Enums;
using ChatBot.Domain.Repositories;
using Microsoft.Extensions.Logging; // Opcional, para logar a seleção da resposta

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratégia que seleciona uma resposta do bot baseada em palavras-chave presentes na mensagem do usuário.
/// Prioriza respostas com mais correspondências de palavras-chave.
/// </summary>
public class KeywordBasedResponseStrategy : IBotResponseStrategy
{
    private readonly IBotResponseRepository _botResponseRepository;
    private readonly ILogger<KeywordBasedResponseStrategy> _logger;

    public KeywordBasedResponseStrategy(IBotResponseRepository botResponseRepository,
                                        ILogger<KeywordBasedResponseStrategy> logger)
    {
        _botResponseRepository = botResponseRepository;
        _logger = logger;
    }

    public async Task<string?> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken)
    {
        // Converte a mensagem do usuário para minúsculas para facilitar a comparação
        var normalizedUserMessage = userMessage.ToLowerInvariant();

        // Obter todas as respostas de tipo 'KeywordBased'
        var keywordResponses = (await _botResponseRepository
            .GetAllAsync(cancellationToken))
            .Where(r => r.Type == BotResponseType.KeywordBased && r.IsActive)
            .ToList();

        if (!keywordResponses.Any())
        {
            _logger.LogWarning("Nenhuma resposta baseada em palavra-chave ativa encontrada no repositório.");
            return null;
        }

        // Encontrar as respostas que contêm palavras-chave da mensagem do usuário
        var matchingResponses = new Dictionary<BotResponse, int>(); // Resposta -> contagem de palavras-chave correspondentes

        foreach (var response in keywordResponses)
        {
            if (string.IsNullOrWhiteSpace(response.Keywords)) continue;

            // Divide as palavras-chave da resposta e verifica quantas correspondem na mensagem do usuário
            var responseKeywords = response.Keywords
                                           .Split(',')
                                           .Select(k => k.Trim().ToLowerInvariant())
                                           .Where(k => !string.IsNullOrWhiteSpace(k))
                                           .ToList();

            var matchCount = 0;
            foreach (var keyword in responseKeywords)
            {
                if (normalizedUserMessage.Contains(keyword))
                {
                    matchCount++;
                }
            }

            if (matchCount > 0)
            {
                matchingResponses.Add(response, matchCount);
            }
        }

        if (!matchingResponses.Any())
        {
            _logger.LogInformation("Nenhuma resposta baseada em palavra-chave corresponde à mensagem do usuário: {UserMessage}", userMessage);
            return null; // Nenhuma correspondência de palavra-chave
        }

        // Selecionar a resposta com a maior contagem de palavras-chave correspondentes
        // Se houver empate, a prioridade do BotResponse pode ser usada como critério de desempate
        var bestMatch = matchingResponses
            .OrderByDescending(kv => kv.Value) // Mais correspondências primeiro
            .ThenByDescending(kv => kv.Key.Priority) // Maior prioridade em caso de empate
            .FirstOrDefault();

        _logger.LogInformation("Resposta baseada em palavra-chave selecionada: {Content} (Correspondências: {Matches}, Prioridade: {Priority})",
                               bestMatch.Key.Content, bestMatch.Value, bestMatch.Key.Priority);
        return bestMatch.Key.Content;
    }
}