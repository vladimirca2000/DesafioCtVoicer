using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;

namespace ChatBot.Application.Features.Chat.Queries.GetActiveSessions;

/// <summary>
/// Handler para buscar a sessão ativa de um usuário específico
/// </summary>
public class GetActiveChatSessionQueryHandler : IRequestHandler<GetActiveChatSessionQuery, Result<ActiveSessionDto?>>
{
    private readonly IChatSessionRepository _chatSessionRepository;

    public GetActiveChatSessionQueryHandler(IChatSessionRepository chatSessionRepository)
    {
        _chatSessionRepository = chatSessionRepository;
    }

    public async Task<Result<ActiveSessionDto?>> Handle(GetActiveChatSessionQuery request, CancellationToken cancellationToken)
    {
        // Buscar todas as sessões do usuário
        var userSessions = await _chatSessionRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        // Filtrar apenas as sessões ativas
        var activeSession = userSessions.FirstOrDefault(s => s.Status == SessionStatus.Active);
        
        if (activeSession == null)
        {
            return Result<ActiveSessionDto?>.Success(null);
        }

        var dto = new ActiveSessionDto
        {
            ChatSessionId = activeSession.Id,
            UserId = activeSession.UserId,
            StartedAt = activeSession.StartedAt,
            Status = activeSession.Status.ToString(),
            MessageCount = activeSession.Messages?.Count ?? 0
        };

        return Result<ActiveSessionDto?>.Success(dto);
    }
}
