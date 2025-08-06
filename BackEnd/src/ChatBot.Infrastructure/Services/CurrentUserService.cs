using ChatBot.Application.Common.Interfaces;

namespace ChatBot.Infrastructure.Services;


public class CurrentUserService : ICurrentUserService
{
    public Guid UserId => throw new NotImplementedException();

    public string UserName => throw new NotImplementedException();

    public bool IsAuthenticated => throw new NotImplementedException();

    public string? GetCurrentUserIdentifier()
    {
        
        return "system_user_id"; 
    }

    public string? GetCurrentUserName()
    {
        
        return "System"; 
    }

    public bool IsUserAuthenticated()
    {
        return true;
    }
}