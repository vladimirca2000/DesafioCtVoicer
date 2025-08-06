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
        
    }

    /// <summary>
    /// Permite que um cliente saia de um grupo de chat (sessão).
    /// </summary>
    /// <param name="chatSessionId">O ID da sessão de chat.</param>
    public async Task LeaveChat(string chatSessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatSessionId);
    }

}
