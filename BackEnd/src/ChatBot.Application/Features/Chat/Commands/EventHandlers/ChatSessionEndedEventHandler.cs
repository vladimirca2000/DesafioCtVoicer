using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Features.Chat.EventHandlers;

/// <summary>
/// Manipulador para o evento de domínio ChatSessionEndedDomainEvent.
/// Notifica os clientes via SignalR sobre o encerramento da sessão.
/// </summary>
public class ChatSessionEndedEventHandler : INotificationHandler<ChatSessionEndedDomainEvent>
{
    private readonly ISignalRChatService _signalRChatService;
    private readonly ILogger<ChatSessionEndedEventHandler> _logger;

    public ChatSessionEndedEventHandler(ISignalRChatService signalRChatService, ILogger<ChatSessionEndedEventHandler> logger)
    {
        _signalRChatService = signalRChatService;
        _logger = logger;
    }

    public async Task Handle(ChatSessionEndedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Domain Event: ChatSessionEndedDomainEvent recebido para ChatSessionId: {ChatSessionId}", notification.ChatSessionId);

        await _signalRChatService.NotifyChatSessionEndedAsync(notification.ChatSessionId, notification.EndReason ?? "Sessão encerrada.", cancellationToken);

        _logger.LogInformation("Notificação de encerramento de sessão enviada via SignalR para a sessão {ChatSessionId}", notification.ChatSessionId);
    }
}