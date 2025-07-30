namespace ChatBot.Application.Features.Chat.StartChatSession;

public record StartChatSessionResponse
{
    public Guid ChatSessionId { get; init; }
    public Guid UserId { get; init; }
    public DateTime StartedAt { get; init; }
    public string InitialMessage { get; init; } = string.Empty;
}