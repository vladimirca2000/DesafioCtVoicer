namespace ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;

/// <summary>
/// Resposta gerada pelo bot.
/// </summary>
public record ProcessUserMessageResponse
{
    public Guid MessageId { get; init; }
    public Guid ChatSessionId { get; init; }
    public string BotResponseContent { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
}