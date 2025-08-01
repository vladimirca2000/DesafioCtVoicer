using ChatBot.Application.Common.Interfaces;
using ChatBot.Shared.DTOs.Chat;
using Microsoft.AspNetCore.SignalR; // Adicionar este using
using ChatBot.Api.Hubs;

namespace ChatBot.Api.Services;

/// <summary>
/// Implementação do serviço de notificação em tempo real via SignalR.
/// </summary>
public class SignalRChatService : ISignalRChatService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRChatService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageToChatSessionAsync(Guid chatSessionId, ChatMessageDto message, CancellationToken cancellationToken)
    {
        // Envia a mensagem para um grupo específico (a sessão de chat)
        await _hubContext.Clients.Group(chatSessionId.ToString()).SendAsync("ReceiveMessage", message, cancellationToken);
    }

    public async Task NotifyChatSessionEndedAsync(Guid chatSessionId, string reason, CancellationToken cancellationToken)
    {
        // Notifica um grupo específico sobre o encerramento da sessão
        await _hubContext.Clients.Group(chatSessionId.ToString()).SendAsync("SessionEnded", chatSessionId, reason, cancellationToken);
    }
}