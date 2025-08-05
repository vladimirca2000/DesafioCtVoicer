using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Features.Users.Queries.GetUserById; // Para usar UserDetailDto

namespace ChatBot.Application.Features.Users.Queries.GetUserByEmail;

/// <summary>
/// Query para obter os detalhes de um usurio por seu endereo de e-mail.
/// </summary>
public record GetUserByEmailQuery : IQuery<Result<UserDetailDto>>
{
    public string Email { get; init; } = string.Empty;
}