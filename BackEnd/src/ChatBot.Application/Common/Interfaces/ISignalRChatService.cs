// Conteúdo COMPLETO e CORRETO para ISignalRChatService.cs
using System;
using System.Threading.Tasks;

namespace ChatBot.Application.Common.Interfaces;

/// <summary>
/// Contrato para o serviço de comunicação de chat em tempo real via SignalR.
/// </summary>
public interface ISignalRChatService
{
    /// <summary>
    /// Envia uma nova mensagem para todos os clientes em uma sessão de chat específica.
    /// </summary>
    /// <param name="chatSessionId">ID da sessão de chat.</param>
    /// <param name="messageContent">Conteúdo da mensagem.</param>
    /// <param name="isBot">Indica se a mensagem é do bot.</param>
    /// <param name="userId">ID do usuário (nulo se for do bot).</param>
    /// <param name="messageId">ID único da mensagem.</param>
    /// <param name="sentAt">Timestamp da mensagem.</param>
    Task SendMessageToChatSession(Guid chatSessionId, string messageContent, bool isBot, Guid? userId, Guid messageId, DateTime sentAt);

    /// <summary>
    /// Notifica os clientes que uma sessão de chat foi encerrada.
    /// </summary>
    /// <param name="chatSessionId">ID da sessão de chat.</param>
    /// <param name="reason">Motivo do encerramento.</param>
    Task NotifyChatSessionEnded(Guid chatSessionId, string reason);
}