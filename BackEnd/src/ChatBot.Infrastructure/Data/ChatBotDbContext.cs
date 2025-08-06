using Microsoft.EntityFrameworkCore;
using ChatBot.Domain.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics; 

namespace ChatBot.Infrastructure.Data;

public class ChatBotDbContext : DbContext
{
    
    public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options) : base(options)
    {
        
    }

    
    public DbSet<User> Users => Set<User>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<BotResponse> BotResponses => Set<BotResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatBotDbContext).Assembly);

        
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ChatSession>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Message>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BotResponse>().HasQueryFilter(e => !e.IsDeleted);
    }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        
        optionsBuilder.UseNpgsql(options =>
        {
            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }
}