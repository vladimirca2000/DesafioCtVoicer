namespace ChatBot.Application.Features.Users.Commands.UpdateUserStatus;

/// <summary>
/// Resposta para o comando de atualização de status de usuário.
/// </summary>
public record UpdateUserStatusResponse
{
    public Guid UserId { get; init; }
    public bool IsActive { get; init; }
    public DateTime UpdatedAt { get; init; }
}