using Microsoft.EntityFrameworkCore;
using ChatBot.Domain.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics; // Necessário para ISaveChangesInterceptor

namespace ChatBot.Infrastructure.Data;

public class ChatBotDbContext : DbContext
{
    // Construtor que recebe DbContextOptions e IEnumerable<ISaveChangesInterceptor>
    public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options) : base(options)
    {
        // Os interceptors agora são adicionados via AddDbContext na classe DependencyInjection.cs
        // Não é mais necessário adicioná-los aqui diretamente
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<BotResponse> BotResponses => Set<BotResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatBotDbContext).Assembly);

        // Global Query Filters (Soft Delete)
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ChatSession>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Message>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BotResponse>().HasQueryFilter(e => !e.IsDeleted);
    }

    // O método OnConfiguring não precisa mais adicionar os interceptors diretamente,
    // pois eles são adicionados através do construtor via DI.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Configuração para PostgreSQL case-sensitivity
        optionsBuilder.UseNpgsql(options =>
        {
            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }
}