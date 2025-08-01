using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;
using System.Collections.Generic;

namespace ChatBot.Application.Features.Users.Queries.GetUserSessions;

/// <summary>
/// Query para obter todas as sessões de chat de um usuário específico.
/// </summary>
public record GetUserSessionsQuery : IQuery<Result<IEnumerable<UserSessionDto>>>
{
    public Guid UserId { get; init; }
}