using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;

namespace ChatBot.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Comando para realizar o soft delete de um usuário.
/// </summary>
public record DeleteUserCommand : ICommand<Result<DeleteUserResponse>>
{
    public Guid UserId { get; init; }
}