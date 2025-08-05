// File: ChatBot.Infrastructure/Services/CurrentUserService.cs
using ChatBot.Application.Common.Interfaces;

namespace ChatBot.Infrastructure.Services;

// Implementação inicial dummy do serviço de usuário atual.
// Em uma aplicação real, isto obteria o usuário do HttpContext, etc.
public class CurrentUserService : ICurrentUserService
{
    public Guid UserId => throw new NotImplementedException();

    public string UserName => throw new NotImplementedException();

    public bool IsAuthenticated => throw new NotImplementedException();

    public string? GetCurrentUserIdentifier()
    {
        // TODO: Implementar lógica para obter o ID do usuário autenticado (ex: do HttpContext.User.Claims)
        return "system_user_id"; // Valor padrão para testes
    }

    public string? GetCurrentUserName()
    {
        // TODO: Implementar lógica para obter o nome do usuário autenticado
        return "System"; // Valor padrão para testes
    }

    public bool IsUserAuthenticated()
    {
        return true;
    }
}