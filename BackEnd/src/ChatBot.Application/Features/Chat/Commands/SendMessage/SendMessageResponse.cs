namespace ChatBot.Application.Features.Chat.Commands.SendMessage;

public record SendMessageResponse
{
    public Guid MessageId { get; init; }
    public Guid ChatSessionId { get; init; }
    public Guid UserId { get; init; }
    public string Content { get; init; } = string.Empty; // Permanece string para output da API
    public DateTime SentAt { get; init; }
}