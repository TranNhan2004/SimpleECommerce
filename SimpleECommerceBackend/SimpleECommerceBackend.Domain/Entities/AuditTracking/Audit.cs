using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Domain.Entities.AuditTracking;

public class Audit : EntityBase
{
    public Audit()
    {
    }

    private string _entityName = null!;
    private Guid _entityId;
    private AuditOperationType _operationType;
    private string _traceId = null!;
    private string _ipAddress = null!;
    private string? _oldValues;
    private string? _newValues;
    private DateTimeOffset _auditedAt;
    private string _auditedBy = null!;

    public string EntityName
    {
        get => _entityName;
        set => _entityName = value ?? throw new ArgumentNullException(nameof(EntityName));
    }

    public Guid EntityId
    {
        get => _entityId;
        set => _entityId = value;
    }

    public AuditOperationType OperationType
    {
        get => _operationType;
        set => _operationType = value;
    }

    public string TraceId
    {
        get => _traceId;
        set => _traceId = value ?? throw new ArgumentNullException(nameof(TraceId));
    }

    public string IpAddress
    {
        get => _ipAddress;
        set => _ipAddress = value ?? throw new ArgumentNullException(nameof(IpAddress));
    }

    public string? OldValues
    {
        get => _oldValues;
        set => _oldValues = value;
    }

    public string? NewValues
    {
        get => _newValues;
        set => _newValues = value;
    }

    public DateTimeOffset AuditedAt
    {
        get => _auditedAt;
        set => _auditedAt = value;
    }

    public string AuditedBy
    {
        get => _auditedBy;
        set => _auditedBy = value ?? throw new ArgumentNullException(nameof(AuditedBy));
    }
}
