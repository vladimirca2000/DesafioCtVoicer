namespace ChatBot.Application.Features.Chat.Commands.EndChatSession;

/// <summary>
/// Resposta para o comando de encerrar uma sessão de chat.
/// </summary>
public record EndChatSessionResponse
{
    public Guid ChatSessionId { get; init; }
    public DateTime EndedAt { get; init; }
    public string Reason { get; init; } = string.Empty;
}