using System.Security.Claims;
using ChatBot.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http; 

namespace ChatBot.Api.Services;

/// <summary>
/// Implementação de ICurrentUserService que obtém o contexto do usuário a partir do HttpContext.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Obtém o ID do usuário a partir das claims (se autenticado).
    /// Retorna Guid.Empty se não autenticado ou se a claim de ID não for encontrada.
    /// </summary>
    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Obtém o nome do usuário a partir das claims (se autenticado).
    /// Retorna "Anonymous" se não autenticado ou se a claim de nome não for encontrada.
    /// </summary>
    public string UserName
    {
        get
        {
            var userNameClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ??
                                _httpContextAccessor.HttpContext?.User?.FindFirstValue("name"); 

            return userNameClaim ?? (IsAuthenticated ? "AuthenticatedUser" : "Anonymous");
        }
    }

    /// <summary>
    /// Indica se o usuário atual está autenticado.
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}