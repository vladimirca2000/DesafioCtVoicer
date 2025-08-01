using ChatBot.Domain.Interfaces;

namespace ChatBot.Domain.Events;

/// <summary>
/// Evento de domínio disparado quando uma sessão de chat é encerrada.
/// </summary>
public class ChatSessionEndedDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid ChatSessionId { get; }
    public DateTime EndedAt { get; }
    public string? EndReason { get; }

    public ChatSessionEndedDomainEvent(Guid chatSessionId, DateTime endedAt, string? endReason)
    {
        ChatSessionId = chatSessionId;
        EndedAt = endedAt;
        EndReason = endReason;
    }
}