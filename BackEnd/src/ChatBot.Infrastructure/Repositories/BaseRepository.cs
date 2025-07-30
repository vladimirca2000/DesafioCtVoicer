using ChatBot.Domain.Entities;
using ChatBot.Domain.Interfaces;
using ChatBot.Domain.Repositories;
using ChatBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly ChatBotDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(ChatBotDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // O Global Query Filter no DbContext já garante que registros deletados não sejam retornados
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            // O SoftDeleteInterceptor no DbContext vai interceptar essa operação e marcar IsDeleted como true
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is ISoftDeletable softDeletableEntity && softDeletableEntity.IsDeleted)
        {
            softDeletableEntity.IsDeleted = false;
            softDeletableEntity.DeletedAt = null;
            softDeletableEntity.DeletedBy = null;
            _dbSet.Update(entity);
        }
    }
}