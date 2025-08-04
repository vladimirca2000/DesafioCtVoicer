using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ChatBot.Domain.ValueObjects; // Necessário para Email

namespace ChatBot.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ChatBotDbContext context) : base(context) { }

    public async Task<User?> GetUserByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        // Usamos email.Value para comparar com a coluna string no banco de dados
        return await _dbSet.FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
    }
}