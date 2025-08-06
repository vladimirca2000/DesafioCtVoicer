using ChatBot.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot.Domain.Repositories;

public interface IMessageRepository : IBaseRepository<Message>
{
    Task<Message?> GetLastMessageInSessionAsync(Guid chatSessionId, CancellationToken cancellationToken = default);
}