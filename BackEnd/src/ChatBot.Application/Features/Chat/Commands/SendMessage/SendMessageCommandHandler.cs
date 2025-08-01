using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Events; // Adicionar este using

namespace ChatBot.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageCommandHandler(
        IChatSessionRepository chatSessionRepository,
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _chatSessionRepository = chatSessionRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SendMessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession == null)
        {
            return Result<SendMessageResponse>.Failure("Sessão de chat não encontrada.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<SendMessageResponse>.Failure("Usuário remetente não encontrado.");
        }

        if (chatSession.Status != Domain.Enums.SessionStatus.Active)
        {
            return Result<SendMessageResponse>.Failure("Não é possível enviar mensagens para uma sessão de chat inativa.");
        }

        var message = new Message
        {
            ChatSessionId = request.ChatSessionId,
            UserId = request.UserId,
            Content = request.Content,
            Type = request.MessageType,
            IsFromBot = false,
            SentAt = DateTime.UtcNow,
            CreatedBy = user.Name
        };

        await _messageRepository.AddAsync(message, cancellationToken);

        // Adicionar o evento de domínio após a mensagem ser criada e adicionada ao repositório.
        // O evento será disparado após o SaveChangesAsync no UnitOfWork.
        message.AddDomainEvent(new MessageSentDomainEvent(
            message.Id,
            message.ChatSessionId,
            message.UserId,
            message.Content,
            message.SentAt,
            message.IsFromBot
        ));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SendMessageResponse>.Success(new SendMessageResponse
        {
            MessageId = message.Id,
            ChatSessionId = message.ChatSessionId,
            UserId = message.UserId.Value,
            Content = message.Content,
            SentAt = message.SentAt
        });
    }
}