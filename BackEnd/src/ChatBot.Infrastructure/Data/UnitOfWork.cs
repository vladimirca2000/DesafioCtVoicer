using ChatBot.Application.Common.Interfaces;
using ChatBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using ChatBot.Domain.Interfaces; // Necessário para IDomainEvent
using ChatBot.Domain.Entities; // Necessário para BaseEntity
using System.Linq;
using System.Collections.Generic;

namespace ChatBot.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ChatBotDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(ChatBotDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return; // Transaction already started
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No transaction has been started.");
        }

        try
        {
            // SaveChangesAsync já foi chamado pelo TransactionBehavior antes do Commit
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            return; // No transaction to rollback
        }
        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    // Implementação dos novos métodos para Domain Events
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        // Coleta todos os eventos de domínio das entidades que estão sendo rastreadas pelo DbContext
        return _context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
    }

    public void ClearDomainEvents()
    {
        // Limpa os eventos de domínio de todas as entidades rastreadas após a publicação
        _context.ChangeTracker.Entries<BaseEntity>()
            .ToList() // .ToList() para evitar modificação da coleção durante a iteração
            .ForEach(e => e.Entity.ClearDomainEvents());
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
        }

        await _context.DisposeAsync();
    }
}