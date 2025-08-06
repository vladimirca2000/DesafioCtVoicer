using ChatBot.Domain.Interfaces;
using System;

namespace ChatBot.Domain.Events;

/// <summary>
/// Evento de domínio disparado quando uma mensagem é enviada (salva no banco).
/// </summary>
public record MessageSentDomainEvent : IDomainEvent
{
    public Guid MessageId { get; init; }
    public Guid ChatSessionId { get; init; }
    public Guid? UserId { get; init; } 
    public string Content { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
    public bool IsFromBot { get; init; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

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