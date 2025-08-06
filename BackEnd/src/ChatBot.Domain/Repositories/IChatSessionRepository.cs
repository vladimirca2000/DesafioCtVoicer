using ChatBot.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot.Domain.Repositories;

public interface IChatSessionRepository : IBaseRepository<ChatSession>
{
   
    Task<IEnumerable<ChatSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
}