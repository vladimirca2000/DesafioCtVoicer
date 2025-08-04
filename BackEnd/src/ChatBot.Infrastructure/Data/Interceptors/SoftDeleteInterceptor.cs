using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ChatBot.Domain.Interfaces;
using ChatBot.Application.Common.Interfaces; // Necessário para ICurrentUserService

namespace ChatBot.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor para implementar o soft delete (exclusão lógica) automaticamente.
/// </summary>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    // Construtor para injeção de dependência
    public SoftDeleteInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateSoftDeleteStatuses(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateSoftDeleteStatuses(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateSoftDeleteStatuses(DbContext? context)
    {
        if (context == null) return;

        var currentUserName = _currentUserService.UserName;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is not { Entity: ISoftDeletable entity }) continue;

            switch (entry.State)
            {
                case EntityState.Deleted:
                    entry.State = EntityState.Modified; // Muda o estado para Modified em vez de Deleted
                    entity.IsDeleted = true;
                    entity.DeletedAt = DateTime.UtcNow;
                    entity.DeletedBy = currentUserName; // Usar o nome do usuário logado
                    break;
            }
        }
    }
}