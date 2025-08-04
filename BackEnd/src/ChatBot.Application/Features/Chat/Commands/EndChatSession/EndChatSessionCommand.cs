using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;

namespace ChatBot.Application.Features.Chat.Commands.EndChatSession;

/// <summary>
/// Comando para encerrar uma sessão de chat.
/// </summary>
public record EndChatSessionCommand : ICommand<Result<EndChatSessionResponse>>
{
    public Guid ChatSessionId { get; init; }
    public string EndReason { get; init; } = string.Empty;
}