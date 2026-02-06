using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.Interfaces.Time;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Interceptors;

[AutoConstructor]
public sealed partial class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IClock _clock;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context == null) return;

        var now = _clock.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added &&
                entry.Entity is ICreatedTime created)
                entry.Property(nameof(ICreatedTime.CreatedAt)).CurrentValue = now;

            if (entry.State == EntityState.Modified &&
                entry.Entity is IUpdatedTime updated)
                entry.Property(nameof(IUpdatedTime.UpdatedAt)).CurrentValue = now;
        }
    }
}