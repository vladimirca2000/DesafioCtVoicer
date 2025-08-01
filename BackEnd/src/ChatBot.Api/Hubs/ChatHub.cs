using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatBot.Api.Hubs;

/// <summary>
/// Hub SignalR para o chat.
/// Lida com as conexões e comunicação em tempo real com os clientes.
/// </summary>
public class ChatHub : Hub
{
    /// <summary>
    /// Permite que um cliente se junte a um grupo de chat (sessão).
    /// </summary>
    /// <param name="chatSessionId">O ID da sessão de chat.</param>
    public async Task JoinChat(string chatSessionId) // Usar string para o ID do grupo SignalR
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatSessionId);
        // Opcional: Enviar uma mensagem de boas-vindas ao usuário que acabou de entrar
        // await Clients.Caller.SendAsync("ReceiveMessage", new { Content = $"Bem-vindo à sessão {chatSessionId}!" });
    }

    /// <summary>
    /// Permite que um cliente saia de um grupo de chat (sessão).
    /// </summary>
    /// <param name="chatSessionId">O ID da sessão de chat.</param>
    public async Task LeaveChat(string chatSessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatSessionId);
    }

    // Métodos que o servidor pode chamar nos clientes (definidos no ISignalRChatService)
    // Eles não precisam de implementação aqui, apenas a assinatura é suficiente para o SignalR.
    // O SendAsync no SignalRChatService chama esses métodos pelo nome.
}
