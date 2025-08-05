using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Events; // Necessário para ChatSessionEndedDomainEvent

namespace ChatBot.Application.Features.Chat.Commands.EndChatSession;

/// <summary>
/// Manipulador para o comando EndChatSessionCommand.
/// Encerra uma sessão de chat e dispara um evento de domínio.
/// </summary>
public class EndChatSessionCommandHandler : IRequestHandler<EndChatSessionCommand, Result<EndChatSessionResponse>>
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EndChatSessionCommandHandler(IChatSessionRepository chatSessionRepository, IUnitOfWork unitOfWork)
    {
        _chatSessionRepository = chatSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EndChatSessionResponse>> Handle(EndChatSessionCommand request, CancellationToken cancellationToken)
    {
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession == null)
        {
            return Result<EndChatSessionResponse>.Failure($"Sessão de chat com ID '{request.ChatSessionId}' não foi encontrada.");
        }

        if (chatSession.Status == SessionStatus.Ended)
        {
            return Result<EndChatSessionResponse>.Failure($"Esta sessão de chat já está encerrada desde {chatSession.EndedAt:dd/MM/yyyy HH:mm}.");
        }

        chatSession.Status = SessionStatus.Ended;
        chatSession.EndedAt = DateTime.UtcNow;
        chatSession.EndReason = request.EndReason;
        chatSession.UpdatedBy = "System"; // Ou obter do contexto do usuário

        await _chatSessionRepository.UpdateAsync(chatSession, cancellationToken);

        // Adiciona o evento de domínio que será publicado após o SaveChangesAsync
        chatSession.AddDomainEvent(new ChatSessionEndedDomainEvent(
            chatSession.Id,
            chatSession.EndReason ?? "Sessão encerrada pelo usuário."
        ));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<EndChatSessionResponse>.Success(new EndChatSessionResponse
        {
            ChatSessionId = chatSession.Id,
            EndedAt = chatSession.EndedAt.Value,
            Reason = chatSession.EndReason ?? "N/A"
        });
    }
}