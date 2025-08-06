using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using ChatBot.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Features.Bot.Strategies;

public class KeywordBasedResponseStrategy : IBotResponseStrategy
{
    private readonly IBotResponseRepository _botResponseRepository;
    private readonly ILogger<KeywordBasedResponseStrategy> _logger;

    public KeywordBasedResponseStrategy(IBotResponseRepository botResponseRepository, ILogger<KeywordBasedResponseStrategy> logger)
    {
        _botResponseRepository = botResponseRepository;
        _logger = logger;
    }

    public async Task<bool> CanHandle(ProcessUserMessageCommand command)
    {
        var message = NormalizeMessage(command.UserMessage);
        var keywordResponses = (await _botResponseRepository.GetAllAsync())
            .Where(r => r.Type == BotResponseType.KeywordBased && r.IsActive && !r.IsDeleted && !string.IsNullOrEmpty(r.Keywords))
            .ToList();

        foreach (var response in keywordResponses)
        {
            var responseKeywords = response.Keywords!.Split(',')
                .Select(k => NormalizeMessage(k.Trim()))
                .Where(k => !string.IsNullOrEmpty(k));
            if (responseKeywords.Any(keyword => message.Contains(keyword)))
                return true;
        }
        return false;
    }

    private string NormalizeMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return string.Empty;
            
        return message.ToLowerInvariant()
            .Replace("á", "a").Replace("à", "a").Replace("ã", "a").Replace("â", "a")
            .Replace("é", "e").Replace("ê", "e")
            .Replace("í", "i")
            .Replace("ó", "o").Replace("ô", "o").Replace("õ", "o")
            .Replace("ú", "u").Replace("ü", "u")
            .Replace("ç", "c")
            .Trim();
    }

    public async Task<MessageContent> GenerateResponse(ProcessUserMessageCommand command)
    {
        var message = NormalizeMessage(command.UserMessage);
        var messageWords = message.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        
        _logger.LogInformation("Analisando mensagem: '{Message}' com {WordCount} palavras", command.UserMessage, messageWords.Count);
        
        var keywordResponses = (await _botResponseRepository.GetAllAsync())
            .Where(r => r.Type == BotResponseType.KeywordBased && r.IsActive && !r.IsDeleted && !string.IsNullOrEmpty(r.Keywords))
            .ToList();

        _logger.LogInformation("Encontradas {Count} respostas baseadas em palavras-chave", keywordResponses.Count);

        var matches = new List<(BotResponse response, double score, string matchType)>();

        foreach (var response in keywordResponses)
        {
            var responseKeywords = response.Keywords!.Split(',')
                .Select(k => NormalizeMessage(k.Trim()))
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList();

            var contextualScore = CalculateContextualScore(message, messageWords, responseKeywords, response);
            
            if (contextualScore.score > 0)
            {
                matches.Add((response, contextualScore.score, contextualScore.matchType));
                _logger.LogInformation("Resposta candidata: '{Content}' - Pontuação: {Score:F2} - Tipo: {MatchType}", 
                    response.Content.Substring(0, Math.Min(50, response.Content.Length)) + "...", 
                    contextualScore.score, contextualScore.matchType);
            }
        }

        if (matches.Any())
        {
            var bestMatch = matches
                .OrderByDescending(m => m.score)
                .ThenBy(m => m.response.Priority)
                .First();
            
            _logger.LogInformation("🎯 Melhor resposta selecionada: '{Content}' - Pontuação: {Score:F2} - Tipo: {MatchType}", 
                bestMatch.response.Content.Substring(0, Math.Min(50, bestMatch.response.Content.Length)) + "...", 
                bestMatch.score, bestMatch.matchType);
            
            return MessageContent.Create(bestMatch.response.Content);
        }

        _logger.LogWarning("Nenhuma correspondência contextual encontrada para a mensagem '{Message}'", command.UserMessage);
        return MessageContent.Create("Não entendi sua pergunta. Posso ajudar com algo mais?");
    }

    private (double score, string matchType) CalculateContextualScore(string message, List<string> messageWords, List<string> responseKeywords, BotResponse response)
    {
        double score = 0;
        var matchTypes = new List<string>();

        var exactMatches = messageWords.Intersect(responseKeywords).ToList();
        if (exactMatches.Any())
        {
            score += exactMatches.Count * 10.0;
            matchTypes.Add($"Exata({exactMatches.Count})");
        }

        var partialMatches = responseKeywords.Where(keyword => message.Contains(keyword) && !exactMatches.Contains(keyword)).ToList();
        if (partialMatches.Any())
        {
            score += partialMatches.Count * 5.0;
            matchTypes.Add($"Parcial({partialMatches.Count})");
        }

        var contextBonus = AnalyzeQuestionContext(message, responseKeywords, response);
        if (contextBonus > 0)
        {
            score += contextBonus;
            matchTypes.Add($"Contexto(+{contextBonus:F1})");
        }

        if (IsSpecificQuestion(message) && IsGenericResponse(response))
        {
            score *= 0.7;
            matchTypes.Add("Penalidade-Genérica");
        }

        var relatedWordsBonus = CalculateRelatedWordsBonus(messageWords, responseKeywords);
        if (relatedWordsBonus > 0)
        {
            score += relatedWordsBonus;
            matchTypes.Add($"Relacionadas(+{relatedWordsBonus:F1})");
        }

        return (score, string.Join(", ", matchTypes));
    }

    private double AnalyzeQuestionContext(string message, List<string> responseKeywords, BotResponse response)
    {
        double bonus = 0;

        if (IsGreeting(message) && ContainsGreetingKeywords(responseKeywords))
        {
            bonus += 3.0;
        }
        else if (IsQuestionAboutPrice(message) && ContainsPriceKeywords(responseKeywords))
        {
            bonus += 5.0;
        }
        else if (IsQuestionAboutHours(message) && ContainsHourKeywords(responseKeywords))
        {
            bonus += 5.0;
        }
        else if (IsQuestionAboutContact(message) && ContainsContactKeywords(responseKeywords))
        {
            bonus += 5.0;
        }
        else if (IsHelpRequest(message) && ContainsHelpKeywords(responseKeywords))
        {
            bonus += 4.0;
        }

        return bonus;
    }

    private bool IsSpecificQuestion(string message)
    {
        var specificWords = new[] { "quanto", "quando", "onde", "como", "qual", "preço", "horário", "contato", "telefone" };
        return specificWords.Any(word => message.Contains(word));
    }

    private bool IsGenericResponse(BotResponse response)
    {
        var genericPhrases = new[] { "como posso ajudar", "em que posso ser útil", "o que gostaria" };
        return genericPhrases.Any(phrase => response.Content.ToLowerInvariant().Contains(phrase));
    }

    private double CalculateRelatedWordsBonus(List<string> messageWords, List<string> responseKeywords)
    {
        var relatedGroups = new Dictionary<string[], double>
        {
            [new[] { "preço", "valor", "custo", "quanto", "custa" }] = 2.0,
            [new[] { "horário", "funcionamento", "aberto", "fechado", "atendimento" }] = 2.0,
            [new[] { "contato", "telefone", "email", "falar", "ligar" }] = 2.0,
            [new[] { "produto", "serviço", "oferta", "venda" }] = 1.5,
            [new[] { "ajuda", "auxilio", "suporte", "apoio" }] = 1.5
        };

        foreach (var group in relatedGroups)
        {
            var messageMatches = messageWords.Intersect(group.Key).Count();
            var keywordMatches = responseKeywords.Intersect(group.Key).Count();
            
            if (messageMatches > 1 && keywordMatches > 0)
            {
                return group.Value * messageMatches;
            }
        }

        return 0;
    }

    private bool IsGreeting(string message) => 
        new[] { "oi", "ola", "olá", "hello", "hi", "bom dia", "boa tarde", "boa noite" }.Any(g => message.Contains(g));

    private bool IsQuestionAboutPrice(string message) => 
        new[] { "preço", "valor", "custo", "quanto", "custa", "barato", "caro" }.Any(p => message.Contains(p));

    private bool IsQuestionAboutHours(string message) => 
        new[] { "horário", "funcionamento", "aberto", "fechado", "quando", "abre", "fecha" }.Any(h => message.Contains(h));

    private bool IsQuestionAboutContact(string message) => 
        new[] { "contato", "telefone", "email", "falar", "ligar", "whatsapp" }.Any(c => message.Contains(c));

    private bool IsHelpRequest(string message) => 
        new[] { "ajuda", "help", "socorro", "auxilio", "preciso", "suporte" }.Any(h => message.Contains(h));

    private bool ContainsGreetingKeywords(List<string> keywords) => 
        keywords.Any(k => new[] { "oi", "ola", "olá", "hello", "hi", "bom dia", "boa tarde", "boa noite" }.Contains(k));

    private bool ContainsPriceKeywords(List<string> keywords) => 
        keywords.Any(k => new[] { "preço", "preços", "valor", "valores", "custo", "custos" }.Contains(k));

    private bool ContainsHourKeywords(List<string> keywords) => 
        keywords.Any(k => new[] { "horário", "horarios", "funcionamento", "atendimento" }.Contains(k));

    private bool ContainsContactKeywords(List<string> keywords) => 
        keywords.Any(k => new[] { "contato", "telefone", "email", "falar", "ligar", "whatsapp" }.Contains(k));

    private bool ContainsHelpKeywords(List<string> keywords) => 
        keywords.Any(k => new[] { "ajuda", "help", "socorro", "auxilio", "suporte", "apoio" }.Contains(k));
}