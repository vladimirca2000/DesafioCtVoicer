using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Events; // Necessário para MessageSentDomainEvent
using ChatBot.Domain.ValueObjects;

namespace ChatBot.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    // private readonly ISignalRChatService _signalRChatService; // REMOVER: Não é mais injetado diretamente

    public SendMessageCommandHandler(
        IChatSessionRepository chatSessionRepository,
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork
        // ISignalRChatService signalRChatService // REMOVER do construtor
        )
    {
        _chatSessionRepository = chatSessionRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        // _signalRChatService = signalRChatService; // REMOVER
    }

    public async Task<Result<SendMessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar se a sessão de chat existe
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession == null)
        {
            throw new NotFoundException("Sessão de chat", request.ChatSessionId);
        }

        // 2. Validar se o usuário existe
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário remetente", request.UserId);
        }

        // Se a sessão estiver encerrada, não permitir o envio de mensagens (opcional, dependendo da regra de negócio)
        if (chatSession.Status != Domain.Enums.SessionStatus.Active)
        {
            throw new BusinessRuleException("Não é possível enviar mensagens para uma sessão de chat inativa.");
        }

        // 3. Criar o Value Object MessageContent a partir da string de entrada.
        var messageContent = MessageContent.Create(request.Content);

        // 4. Criar a nova mensagem
        var message = new Message
        {
            ChatSessionId = request.ChatSessionId,
            UserId = request.UserId,
            Content = messageContent,
            Type = request.MessageType,
            IsFromBot = false, // Esta mensagem é do usuário, não do bot
            SentAt = DateTime.UtcNow,
            CreatedBy = user.Name
        };

        // 5. Adicionar a mensagem ao repositório
        await _messageRepository.AddAsync(message, cancellationToken);

        // 6. Adicionar o evento de domínio. Ele será publicado pelo TransactionBehavior após o SaveChangesAsync.
        message.AddDomainEvent(new MessageSentDomainEvent(
            message.Id,
            message.ChatSessionId,
            message.UserId,
            message.Content.Value, // Passa a string do conteúdo para o evento
            message.SentAt,
            message.IsFromBot
        ));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // REMOVER: A notificação via SignalR agora será feita pelo MessageSentEventHandler
        // await _signalRChatService.SendMessageToChatSession(...);

        // 7. Retornar a resposta de sucesso
        return Result<SendMessageResponse>.Success(new SendMessageResponse
        {
            MessageId = message.Id,
            ChatSessionId = message.ChatSessionId,
            UserId = message.UserId.Value,
            Content = message.Content.Value,
            SentAt = message.SentAt
        });
    }
}