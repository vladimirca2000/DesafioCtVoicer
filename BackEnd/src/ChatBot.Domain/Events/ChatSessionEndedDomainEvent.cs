using ChatBot.Domain.Interfaces;
using System;

namespace ChatBot.Domain.Events;

/// <summary>
/// Evento de domínio disparado quando uma sessão de chat é encerrada.
/// </summary>
public record ChatSessionEndedDomainEvent : IDomainEvent
{
    public Guid ChatSessionId { get; init; }
    public string Reason { get; init; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public ChatSessionEndedDomainEvent(Guid chatSessionId, string reason)
    {
        ChatSessionId = chatSessionId;
        Reason = reason;
    }
}