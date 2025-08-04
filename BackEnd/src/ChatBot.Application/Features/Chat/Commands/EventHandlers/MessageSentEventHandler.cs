// Conteúdo COMPLETO e CORRETO para:
// C:\Desenvolvimento\DesafioCtVoicer\BackEnd\src\ChatBot.Application\Features\Chat\EventHandlers\MessageSentEventHandler.cs

using MediatR;
using ChatBot.Domain.Events;
using ChatBot.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// NAMESPACE CORRETO para esta localização:
namespace ChatBot.Application.Features.Chat.EventHandlers;

/// <summary>
/// Manipulador para o evento de domínio MessageSentDomainEvent.
/// Notifica os clientes via SignalR quando uma nova mensagem é enviada.
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
        _logger.LogInformation("Evento MessageSentDomainEvent recebido para sessão {ChatSessionId}. Mensagem ID: {MessageId}", notification.ChatSessionId, notification.MessageId);

        await _signalRChatService.SendMessageToChatSession(
            notification.ChatSessionId,
            notification.Content,
            notification.IsFromBot,
            notification.UserId,
            notification.MessageId,
            notification.SentAt
        );

        _logger.LogInformation("Notificação de nova mensagem enviada via SignalR para sessão {ChatSessionId}.", notification.ChatSessionId);
    }
}