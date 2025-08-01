using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;

namespace ChatBot.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Comando para criar um novo usuário no sistema.
/// </summary>
public record CreateUserCommand : ICommand<Result<CreateUserResponse>>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true; // Por padrão, o usuário é ativo
}