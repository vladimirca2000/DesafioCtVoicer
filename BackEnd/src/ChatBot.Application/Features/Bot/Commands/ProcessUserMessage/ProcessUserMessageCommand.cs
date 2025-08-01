using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models; // Para usar Result<T>
using MediatR; // Mesmo que ICommand já herde, pode ser útil para clareza

namespace ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;

/// <summary>
/// Comando para o bot processar uma mensagem recebida do usuário e gerar uma resposta.
/// </summary>
public record ProcessUserMessageCommand : ICommand<Result<ProcessUserMessageResponse>>
{
    public Guid ChatSessionId { get; init; }
    public Guid UserId { get; init; } // Opcional, se o bot não precisar do usuário para processar a mensagem
    public string UserMessageContent { get; init; } = string.Empty;
}
