using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore; 
using ChatBot.Domain.Enums;

namespace ChatBot.Infrastructure.Repositories;

public class ChatSessionRepository : BaseRepository<ChatSession>, IChatSessionRepository
{
    public ChatSessionRepository(ChatBotDbContext context) : base(context) { }

    public async Task<IEnumerable<ChatSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Messages) 
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.StartedAt) 
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChatSession>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Status == SessionStatus.Active && !s.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
