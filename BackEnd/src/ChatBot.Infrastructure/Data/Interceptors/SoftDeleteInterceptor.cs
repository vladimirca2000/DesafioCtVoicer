using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ChatBot.Domain.Interfaces;
using ChatBot.Application.Common.Interfaces;

namespace ChatBot.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor para implementar o soft delete (exclusão lógica) automaticamente.
/// </summary>
public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

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
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedAt = DateTime.UtcNow;
                    entity.DeletedBy = currentUserName;
                    break;
            }
        }
    }
}