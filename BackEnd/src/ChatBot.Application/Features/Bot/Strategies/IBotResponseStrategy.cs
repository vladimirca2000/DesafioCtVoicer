using ChatBot.Domain.ValueObjects; // Necessário para MessageContent
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Contrato para as diferentes estratégias de resposta do bot.
/// </summary>
public interface IBotResponseStrategy
{
    /// <summary>
    /// Verifica se esta estratégia pode lidar com o comando de mensagem do usuário.
    /// </summary>
    bool CanHandle(ProcessUserMessageCommand command);

    /// <summary>
    /// Gera o conteúdo da resposta do bot.
    /// </summary>
    MessageContent GenerateResponse(ProcessUserMessageCommand command);
}