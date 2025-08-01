using ChatBot.Application.Common.Interfaces;
using MediatR; // Adicionar este using
using Microsoft.EntityFrameworkCore.Storage;
using ChatBot.Domain.Entities; // Adicionar este using

namespace ChatBot.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ChatBotDbContext _context;
    private readonly IMediator _mediator; // Injetar IMediator
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(ChatBotDbContext context, IMediator mediator) // Adicionar IMediator ao construtor
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Disparar eventos antes de salvar, mas processá-los após o commit
        // ou coletá-los e disparar após o save.
        // A abordagem mais robusta é coletar todos os eventos e dispará-los após o SaveChangesAsync.
        var result = await _context.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(); // Chamada para o novo método

        return result;
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
            await _currentTransaction.CommitAsync(cancellationToken); // O SaveChangesAsync já foi chamado antes do commit aqui.
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

    // Novo método para disparar eventos de domínio
    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(x => x.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
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