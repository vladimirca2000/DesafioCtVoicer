using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;

namespace ChatBot.Application.Features.Users.Commands.UpdateUserStatus;

/// <summary>
/// Comando para atualizar o status (ativo/inativo) de um usuário existente.
/// </summary>
public record UpdateUserStatusCommand : ICommand<Result<UpdateUserStatusResponse>>
{
    public Guid UserId { get; init; }
    public bool IsActive { get; init; }
}