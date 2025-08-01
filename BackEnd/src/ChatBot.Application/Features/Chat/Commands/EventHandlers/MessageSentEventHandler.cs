using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Events;
using ChatBot.Shared.DTOs.Chat; // Para ChatMessageDto
using Microsoft.Extensions.Logging; // Opcional, para logar

namespace ChatBot.Application.Features.Chat.EventHandlers;

/// <summary>
/// Manipulador para o evento de domínio MessageSentDomainEvent.
/// Envia a mensagem para os clientes via SignalR.
/// </summary>
public class MessageSentEventHandler : INotificationHandler<MessageSentDomainEvent>
{
    private readonly ISignalRChatService _signalRChatService;
    private readonly ILogger<MessageSentEventHandler> _logger;

    public MessageSentEventHandler(ISignalRChatService signalRChatService, ILogger<MessageSentEventHandler> logger)
    {
        _signalRChatService = signalRChatService;
        _logger = logger;
    }

    public async Task Handle(MessageSentDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: MessageSentDomainEvent recebido para MessageId: {MessageId}", notification.MessageId);

        var chatMessageDto = new ChatMessageDto
        {
            Id = notification.MessageId,
            ChatSessionId = notification.ChatSessionId,
            UserId = notification.UserId,
            Content = notification.Content,
            SentAt = notification.SentAt,
            IsFromBot = notification.IsFromBot
        };

        await _signalRChatService.SendMessageToChatSessionAsync(notification.ChatSessionId, chatMessageDto, cancellationToken);

        _logger.LogInformation("Mensagem enviada via SignalR para a sessão {ChatSessionId}", notification.ChatSessionId);
    }
}