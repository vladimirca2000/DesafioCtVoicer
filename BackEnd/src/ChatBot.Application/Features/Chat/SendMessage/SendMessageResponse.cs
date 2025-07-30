namespace ChatBot.Application.Features.Chat.SendMessage;

public record SendMessageResponse
{
    public Guid MessageId { get; init; }
    public Guid ChatSessionId { get; init; }
    public Guid UserId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
}
