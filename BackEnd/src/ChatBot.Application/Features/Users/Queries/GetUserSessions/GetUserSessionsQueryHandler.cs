using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace ChatBot.Application.Features.Users.Queries.GetUserSessions;

/// <summary>
/// Manipulador para a query GetUserSessionsQuery.
/// </summary>
public class GetUserSessionsQueryHandler : IRequestHandler<GetUserSessionsQuery, Result<IEnumerable<UserSessionDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IChatSessionRepository _chatSessionRepository;

    public GetUserSessionsQueryHandler(IUserRepository userRepository, IChatSessionRepository chatSessionRepository)
    {
        _userRepository = userRepository;
        _chatSessionRepository = chatSessionRepository;
    }

    public async Task<Result<IEnumerable<UserSessionDto>>> Handle(GetUserSessionsQuery request, CancellationToken cancellationToken)
    {
        // 1. Verificar se o usuário existe (opcional, dependendo da regra de negócio - se a sessão só pode existir para usuário existente)
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<IEnumerable<UserSessionDto>>.Failure($"Usuário com ID '{request.UserId}' não encontrado.");
        }

        // 2. Obter as sessões de chat do usuário
        var sessions = await _chatSessionRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        // 3. Mapear para DTOs
        var sessionDtos = sessions.Select(s => new UserSessionDto
        {
            ChatSessionId = s.Id,
            Status = s.Status,
            StartedAt = s.StartedAt,
            EndedAt = s.EndedAt,
            EndReason = s.EndReason
        }).ToList();

        // 4. Retornar a lista de DTOs de sucesso
        return Result<IEnumerable<UserSessionDto>>.Success(sessionDtos);
    }
}