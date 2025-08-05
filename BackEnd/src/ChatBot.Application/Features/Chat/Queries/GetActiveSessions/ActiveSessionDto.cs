namespace ChatBot.Application.Features.Chat.Queries.GetActiveSessions;

/// <summary>

/// DTO que representa uma sessão ativa de chat
///
/// </summary>
public record ActiveSessionDto
{
    public Guid ChatSessionId { get; init; }
    public Guid UserId { get; init; }
    public DateTime StartedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public int MessageCount { get; init; }
}
