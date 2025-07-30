using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Infrastructure.Data;

namespace ChatBot.Infrastructure.Repositories;

public class BotResponseRepository : BaseRepository<BotResponse>, IBotResponseRepository
{
    public BotResponseRepository(ChatBotDbContext context) : base(context) { }
}