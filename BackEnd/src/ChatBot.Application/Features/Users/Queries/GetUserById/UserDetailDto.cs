namespace ChatBot.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// DTO para os detalhes de um usuário.
/// </summary>
public record UserDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsDeleted { get; init; }
}