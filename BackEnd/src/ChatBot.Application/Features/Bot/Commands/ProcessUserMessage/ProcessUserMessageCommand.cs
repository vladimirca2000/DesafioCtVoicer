using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;

namespace ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;

/// <summary>
/// Comando para o bot processar uma mensagem recebida do usuário e gerar uma resposta.
/// </summary>
public record ProcessUserMessageCommand : ICommand<Result<ProcessUserMessageResponse>>
{
    public Guid ChatSessionId { get; init; }
    public Guid UserId { get; init; } // O ID do usuário que enviou a mensagem
    public string UserMessage { get; init; } = string.Empty; // O conteúdo da mensagem do usuário (string de entrada)
}