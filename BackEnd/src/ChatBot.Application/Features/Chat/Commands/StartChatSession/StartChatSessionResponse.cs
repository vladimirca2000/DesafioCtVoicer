namespace ChatBot.Application.Features.Chat.Commands.StartChatSession;

/// <summary>
/// Representa a resposta de sucesso ao iniciar uma nova sessão de chat.
/// </summary>
public record StartChatSessionResponse
{
    public Guid ChatSessionId { get; init; }
    public Guid UserId { get; init; }
    public DateTime StartedAt { get; init; }
    public string InitialMessage { get; init; } = string.Empty;
}