using ChatBot.Domain.Interfaces;

namespace ChatBot.Domain.Events;

/// <summary>
/// Evento de domínio disparado quando uma mensagem é enviada e persistida com sucesso.
/// </summary>
public class MessageSentDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow; // Conforme a interface
    public Guid MessageId { get; }
    public Guid ChatSessionId { get; }
    public Guid? UserId { get; } // ID do usuário remetente (null para bot)
    public string Content { get; }
    public DateTime SentAt { get; }
    public bool IsFromBot { get; }

    public MessageSentDomainEvent(Guid messageId, Guid chatSessionId, Guid? userId, string content, DateTime sentAt, bool isFromBot)
    {
        MessageId = messageId;
        ChatSessionId = chatSessionId;
        UserId = userId;
        Content = content;
        SentAt = sentAt;
        IsFromBot = isFromBot;
    }
}
