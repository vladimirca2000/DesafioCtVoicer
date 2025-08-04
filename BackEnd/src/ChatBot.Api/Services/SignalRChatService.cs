using Microsoft.AspNetCore.SignalR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Api.Hubs;
using System.Threading.Tasks;
using System;

namespace ChatBot.Api.Services;

/// <summary>
/// Implementação concreta do serviço de chat em tempo real usando SignalR.
/// </summary>
public class SignalRChatService : ISignalRChatService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRChatService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageToChatSession(Guid chatSessionId, string messageContent, bool isBot, Guid? userId, Guid messageId, DateTime sentAt)
    {
        // Envia um DTO estruturado para o cliente SignalR
        // O cliente deve estar conectado ao grupo com o nome da chatSessionId
        await _hubContext.Clients.Group(chatSessionId.ToString()).SendAsync("ReceiveMessage", new
        {
            MessageId = messageId,
            ChatSessionId = chatSessionId,
            Content = messageContent,
            IsFromBot = isBot,
            UserId = userId,
            SentAt = sentAt
        });
    }

    public async Task NotifyChatSessionEnded(Guid chatSessionId, string reason)
    {
        await _hubContext.Clients.Group(chatSessionId.ToString()).SendAsync("ChatSessionEnded", new { chatSessionId, reason });
    }
}