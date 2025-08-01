namespace ChatBot.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Resposta para o comando de exclusão de usuário.
/// </summary>
public record DeleteUserResponse
{
    public Guid UserId { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
}