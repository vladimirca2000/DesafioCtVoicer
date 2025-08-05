using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore; // Para ToListAsync

namespace ChatBot.Infrastructure.Repositories;

public class ChatSessionRepository : BaseRepository<ChatSession>, IChatSessionRepository
{
    public ChatSessionRepository(ChatBotDbContext context) : base(context) { }

    public async Task<IEnumerable<ChatSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Messages) // Incluir mensagens para poder contar
            .Where(s => s.UserId == userId && !s.IsDeleted) // Apenas sessões não deletadas
            .OrderByDescending(s => s.StartedAt) // Mais recentes primeiro
            .ToListAsync(cancellationToken);
    }
}
