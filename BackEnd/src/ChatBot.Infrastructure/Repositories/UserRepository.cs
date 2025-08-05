// C:\Desenvolvimento\DocChatBoot\BackEnd\src\ChatBot.Infrastructure\Repositories\UserRepository.cs

using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ChatBot.Domain.ValueObjects; // Necessário para Email
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ChatBotDbContext context) : base(context) { }

    public async Task<User?> GetUserByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        // Correção aplicada: Compara diretamente os Value Objects 'Email'.
        // O EF Core traduz isso corretamente para a comparação da coluna string no DB.
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}