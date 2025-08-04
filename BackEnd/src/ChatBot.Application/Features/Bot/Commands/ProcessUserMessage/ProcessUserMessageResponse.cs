namespace ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;

/// <summary>
/// Resposta do comando de processamento de mensagem do usuário pelo bot.
/// </summary>
public record ProcessUserMessageResponse
{
    public Guid MessageId { get; init; }
    public Guid ChatSessionId { get; init; }
    public string BotMessageContent { get; init; } = string.Empty; // O conteúdo da resposta do bot (string para o DTO de saída)
    public DateTime SentAt { get; init; }
}