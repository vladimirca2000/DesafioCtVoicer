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
            return Result<SendMessageResponse>.Failure($"Sessão de chat com ID '{request.ChatSessionId}' não foi encontrada.");
        }

        // 2. Validar se o usuário existe
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<SendMessageResponse>.Failure($"Usuário com ID '{request.UserId}' não foi encontrado.");
        }

        // 3. Verificar se a sessão está ativa para permitir o envio de mensagens
        if (chatSession.Status != Domain.Enums.SessionStatus.Active)
        {
            return Result<SendMessageResponse>.Failure($"Não é possível enviar mensagens para uma sessão de chat com status '{chatSession.Status}'. A sessão deve estar ativa.");
        }

        // 4. Criar o Value Object MessageContent a partir da string de entrada.
        MessageContent messageContent;
        try
        {
            messageContent = MessageContent.Create(request.Content);
        }
        catch (ArgumentException ex)
        {
            return Result<SendMessageResponse>.Failure($"Conteúdo da mensagem inválido: {ex.Message}");
        }

        // 5. Criar a nova mensagem
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

        // 6. Adicionar a mensagem ao repositório
        await _messageRepository.AddAsync(message, cancellationToken);

        // 7. Adicionar o evento de domínio. Ele será publicado pelo TransactionBehavior após o SaveChangesAsync.
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

        // 8. Retornar a resposta de sucesso
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