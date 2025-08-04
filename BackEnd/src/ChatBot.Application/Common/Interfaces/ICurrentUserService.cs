namespace ChatBot.Application.Common.Interfaces;

/// <summary>
/// Define um serviço para obter informações sobre o usuário logado atualmente.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtém o ID do usuário logado (pode ser Guid.Empty se não houver usuário logado ou for anônimo).
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Obtém o nome ou identificador do usuário logado (pode ser "Anonymous" ou "System").
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// Indica se um usuário está autenticado.
    /// </summary>
    bool IsAuthenticated { get; }
}