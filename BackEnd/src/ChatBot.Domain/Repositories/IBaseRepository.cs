namespace ChatBot.Domain.Repositories;

// TEntity deve ser uma classe e herdar de BaseEntity para garantir as propriedades de Soft Delete e Auditoria
public interface IBaseRepository<TEntity> where TEntity : Entities.BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default); // Soft Delete
    Task RestoreAsync(Guid id, CancellationToken cancellationToken = default); // Restore Soft Delete
}

