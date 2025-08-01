namespace ChatBot.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Resposta para o comando de criação de usuário.
/// </summary>
public record CreateUserResponse
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
