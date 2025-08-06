using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using System.Threading.Tasks; // Adicionar este using

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Contrato para as diferentes estratgias de resposta do bot.
/// </summary>
public interface IBotResponseStrategy // Esta é a interface
{
    /// <summary>
    /// Verifica se esta estratgia pode lidar com o comando de mensagem do usurio.
    /// </summary>
    Task<bool> CanHandle(ProcessUserMessageCommand command);

    /// <summary>
    /// Gera o contedo da resposta do bot de forma assncrona.
    /// </summary>
    Task<MessageContent> GenerateResponse(ProcessUserMessageCommand command);
}