using ChatBot.Shared.DTOs.Chat; // Usar o DTO para a mensagem

namespace ChatBot.Application.Common.Interfaces;

/// <summary>
/// Contrato para o serviço de notificação em tempo real via SignalR.
/// </summary>
public interface ISignalRChatService
{
    /// <summary>
    /// Envia uma nova mensagem de chat para todos os clientes conectados a uma sessão específica.
    /// </summary>
    /// <param name="chatSessionId">O ID da sessão de chat.</param>
    /// <param name="message">O DTO da mensagem a ser enviada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task SendMessageToChatSessionAsync(Guid chatSessionId, ChatMessageDto message, CancellationToken cancellationToken);

    /// <summary>
    /// Notifica os clientes sobre o encerramento de uma sessão de chat.
    /// </summary>
    /// <param name="chatSessionId">O ID da sessão de chat.</param>
    /// <param name="reason">O motivo do encerramento da sessão.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task NotifyChatSessionEndedAsync(Guid chatSessionId, string reason, CancellationToken cancellationToken);
}