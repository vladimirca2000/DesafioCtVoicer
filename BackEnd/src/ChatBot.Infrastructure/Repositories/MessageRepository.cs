using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Infrastructure.Repositories;

public class MessageRepository : BaseRepository<Message>, IMessageRepository
{
    public MessageRepository(ChatBotDbContext context) : base(context) { }

    public async Task<Message?> GetLastMessageInSessionAsync(Guid chatSessionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.ChatSessionId == chatSessionId)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}