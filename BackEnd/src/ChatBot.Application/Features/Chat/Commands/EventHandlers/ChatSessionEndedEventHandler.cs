// Conteúdo COMPLETO e CORRETO para:
// C:\Desenvolvimento\DesafioCtVoicer\BackEnd\src\ChatBot.Application\Features\Chat\EventHandlers\ChatSessionEndedEventHandler.cs

using MediatR;
using ChatBot.Domain.Events;
using ChatBot.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// NAMESPACE CORRETO para esta localização:
namespace ChatBot.Application.Features.Chat.EventHandlers;

/// <summary>
/// Manipulador para o evento de domínio ChatSessionEndedDomainEvent.
/// Notifica os clientes via SignalR quando uma sessão de chat é encerrada.
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
        _logger.LogInformation("Evento ChatSessionEndedDomainEvent recebido para sessão {ChatSessionId}. Motivo: {Reason}", notification.ChatSessionId, notification.Reason);

        await _signalRChatService.NotifyChatSessionEnded(notification.ChatSessionId, notification.Reason);

        _logger.LogInformation("Notificação de encerramento de sessão enviada via SignalR para sessão {ChatSessionId}.", notification.ChatSessionId);
    }
}