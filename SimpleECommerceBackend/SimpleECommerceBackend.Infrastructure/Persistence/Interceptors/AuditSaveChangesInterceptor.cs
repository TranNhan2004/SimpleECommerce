using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Entities.AuditTracking;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Interceptors;

public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentRequestContext _currentRequestContext;

    public AuditSaveChangesInterceptor(ICurrentRequestContext currentRequestContext)
    {
        _currentRequestContext = currentRequestContext;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        ApplyAudit(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context == null) return;

        var now = DateTimeOffset.UtcNow;
        var auditRecords = new List<Audit>();

        foreach (var entry in context.ChangeTracker.Entries<IEntity>().ToList())
        {
            if (entry.Entity is Audit) continue;

            if (entry.State == EntityState.Added &&
                entry.Entity is ICreatedTrackable)
                entry.Property(nameof(ICreatedTrackable.CreatedAt)).CurrentValue = now;

            if (entry.State == EntityState.Modified &&
                entry.Entity is IUpdatedTrackable)
                entry.Property(nameof(IUpdatedTrackable.UpdatedAt)).CurrentValue = now;

            if (!TryMapOperationType(entry.State, out var operationType))
                continue;

            auditRecords.Add(
                new Audit
                {
                    Id = UuidUtils.CreateV7(),
                    EntityName = entry.Metadata.ClrType.Name,
                    EntityId = entry.Entity.Id,
                    OperationType = operationType,
                    TraceId = _currentRequestContext.TraceId,
                    IpAddress = _currentRequestContext.IpAddress,
                    OldValues = GetOldValues(entry, operationType),
                    NewValues = GetNewValues(entry, operationType),
                    AuditedAt = now,
                    AuditedBy = _currentRequestContext.UserId
                }
            );
        }

        if (auditRecords.Count > 0)
            context.Set<Audit>().AddRange(auditRecords);
    }

    private static string? GetOldValues(EntityEntry<IEntity> entry, AuditOperationType operationType)
    {
        return operationType switch
        {
            AuditOperationType.Add => null,
            AuditOperationType.Update => SerializeValues(GetModifiedPropertyValues(entry, useOriginalValues: true)),
            AuditOperationType.Delete => SerializeValues(GetAllPropertyValues(entry, useOriginalValues: true)),
            _ => null
        };
    }

    private static string? GetNewValues(EntityEntry<IEntity> entry, AuditOperationType operationType)
    {
        return operationType switch
        {
            AuditOperationType.Add => SerializeValues(GetAllPropertyValues(entry, useOriginalValues: false)),
            AuditOperationType.Update => SerializeValues(GetModifiedPropertyValues(entry, useOriginalValues: false)),
            AuditOperationType.Delete => null,
            _ => null
        };
    }

    private static bool TryMapOperationType(EntityState state, out AuditOperationType operationType)
    {
        switch (state)
        {
            case EntityState.Added:
                operationType = AuditOperationType.Add;
                return true;
            case EntityState.Modified:
                operationType = AuditOperationType.Update;
                return true;
            case EntityState.Deleted:
                operationType = AuditOperationType.Delete;
                return true;
            default:
                operationType = default;
                return false;
        }
    }

    private static Dictionary<string, object?> GetModifiedPropertyValues(
        EntityEntry<IEntity> entry,
        bool useOriginalValues
    )
    {
        return entry.Properties
            .Where(property => property.IsModified)
            .ToDictionary(
                property => property.Metadata.Name,
                property => useOriginalValues ? property.OriginalValue : property.CurrentValue
            );
    }

    private static Dictionary<string, object?> GetAllPropertyValues(
        EntityEntry<IEntity> entry,
        bool useOriginalValues
    )
    {
        return entry.Properties
            .ToDictionary(
                property => property.Metadata.Name,
                property => useOriginalValues ? property.OriginalValue : property.CurrentValue
            );
    }

    private static string? SerializeValues(Dictionary<string, object?> values)
    {
        if (values.Count == 0)
            return null;

        return JsonSerializer.Serialize(values);
    }
}
