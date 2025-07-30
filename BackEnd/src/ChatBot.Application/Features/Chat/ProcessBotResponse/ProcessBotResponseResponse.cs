namespace ChatBot.Application.Features.Chat.ProcessBotResponse;

public record ProcessBotResponseResponse
{
    public Guid MessageId { get; init; }
    public Guid ChatSessionId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
}