using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;

namespace ChatBot.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// Query para obter os detalhes de um usuário por seu ID.
/// </summary>
public record GetUserByIdQuery : IQuery<Result<UserDetailDto>>
{
    public Guid UserId { get; init; }
}