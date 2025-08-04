using ChatBot.Domain.ValueObjects; // Necessário para MessageContent
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Domain.Repositories; // Necessário para IBotResponseRepository
using ChatBot.Domain.Enums; // Necessário para BotResponseType
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratégia de resposta do bot baseada em palavras-chave.
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
        // Exemplo: Verifica se a mensagem contém palavras-chave como "ajuda" ou "suporte"
        var message = command.UserMessage.ToLowerInvariant();
        return message.Contains("ajuda") || message.Contains("suporte");
    }

    public MessageContent GenerateResponse(ProcessUserMessageCommand command)
    {
        // Em um cenário real, isso consultaria o banco de dados por respostas baseadas em palavras-chave.
        // O método GetAllAsync é assíncrono, então você precisa lidar com isso.
        // Para simplificar no contexto do Strategy, estamos usando .Result (bloqueante),
        // mas em um serviço real, a injeção do repositório pode ser mais elaborada ou
        // o `GenerateResponse` seria assíncrono.
        var predefinedResponse = _botResponseRepository
            .GetAllAsync()
            .Result // ATENÇÃO: Evite .Result em código assíncrono real para não causar deadlocks.
            .FirstOrDefault(r => r.Type == BotResponseType.KeywordBased &&
                                 (r.Keywords != null && command.UserMessage.ToLowerInvariant().Contains(r.Keywords.ToLowerInvariant())));

        return MessageContent.Create(predefinedResponse?.Content ?? "Não entendi sua pergunta. Posso ajudar com algo mais?");
    }
}