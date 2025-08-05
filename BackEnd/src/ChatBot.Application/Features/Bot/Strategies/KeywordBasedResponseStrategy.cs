// Conteúdo COMPLETO para ChatBot.Application/Features/Bot/Strategies/KeywordBasedResponseStrategy.cs

using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using System.Linq;
using System.Threading.Tasks; // Adicionar este using

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratgia de resposta do bot baseada em palavras-chave.
/// </summary>
public class KeywordBasedResponseStrategy : IBotResponseStrategy
{
    private readonly IBotResponseRepository _botResponseRepository;

    public KeywordBasedResponseStrategy(IBotResponseRepository botResponseRepository)
    {
        _botResponseRepository = botResponseRepository;
    }

    public bool CanHandle(ProcessUserMessageCommand command)
    {
        // Exemplo: Verifica se a mensagem contm palavras-chave como "ajuda" ou "suporte"
        var message = command.UserMessage.ToLowerInvariant();
        return message.Contains("ajuda") || message.Contains("suporte");
    }

    // ALTERADO: Agora é assíncrono e usa await para GetAllAsync()
    public async Task<MessageContent> GenerateResponse(ProcessUserMessageCommand command)
    {
        // Em um cenrio real, isso consultaria o banco de dados por respostas baseadas em palavras-chave.
        var predefinedResponse = (await _botResponseRepository.GetAllAsync()) // ALTERADO: Removido .Result, adicionado await
            .FirstOrDefault(r => r.Type == BotResponseType.KeywordBased &&
                                 (r.Keywords != null && command.UserMessage.ToLowerInvariant().Contains(r.Keywords.ToLowerInvariant())));

        return MessageContent.Create(predefinedResponse?.Content ?? "No entendi sua pergunta. Posso ajudar com algo mais?");
    }
}