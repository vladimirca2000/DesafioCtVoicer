using MediatR;
using ChatBot.Application.Common.Models;

namespace ChatBot.Application.Features.Chat.Queries.GetActiveSessions;

/// <summary>
/// Query para buscar a sessão ativa de um usuário específico
/// </summary>
public record GetActiveChatSessionQuery : IRequest<Result<ActiveSessionDto?>>
{
    public Guid UserId { get; init; }
}
