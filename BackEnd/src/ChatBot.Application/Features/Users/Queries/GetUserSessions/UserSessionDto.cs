using ChatBot.Domain.Enums; // Para SessionStatus

namespace ChatBot.Application.Features.Users.Queries.GetUserSessions;

/// <summary>
/// DTO para um resumo de sessão de chat de um usuário.
/// </summary>
public record UserSessionDto
{
    public Guid ChatSessionId { get; init; }
    public SessionStatus Status { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
    public string? EndReason { get; init; }
}