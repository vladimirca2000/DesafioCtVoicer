using ChatBot.Domain.Entities;
using ChatBot.Domain.ValueObjects; 
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserByEmailAsync(Email email, CancellationToken cancellationToken = default); 
}