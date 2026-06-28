using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.AuditTracking;

public class Audit : IEntity
{
    public Audit()
    {
    }

    private Guid _id;
    private string _entityName = null!;
    private Guid _entityId;
    private AuditOperationType _operationType;
    private string _traceId = null!;
    private string _ipAddress = null!;
    private string? _oldValues;
    private string? _newValues;
    private DateTimeOffset _auditedAt;
    private Guid _auditedById;
    public User? AuditedBy { get; set; }

    public Guid Id
    {
        get => _id;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    EntityErrorCodes.EmptyId,
                    "Id cannot be empty.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Id"
                    }
                );

            _id = value;
        }
    }

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

    public Guid AuditedById
    {
        get => _auditedById;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    AuditErrorCodes.EmptyAuditById,
                    "AuditedById cannot be empty.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "AuditedById"
                    }
                );

            _auditedById = value;
        }
    }
}
