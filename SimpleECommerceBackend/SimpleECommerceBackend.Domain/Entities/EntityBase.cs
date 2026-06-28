using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities;

public class EntityBase : IEntity, ISoftDeleteTrackable
{
    private Guid _id;
    private DateTimeOffset _createdAt;
    private Guid _createdById;
    private DateTimeOffset? _updatedAt;
    private Guid? _updatedById;
    private bool _isDeleted;
    private DateTimeOffset? _deletedAt;
    private Guid? _deletedById;

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

    public DateTimeOffset CreatedAt
    {
        get => _createdAt;
        set
        {
            if (value == default)
                throw new ValidationException(
                    EntityErrorCodes.InvalidCreatedAt,
                    "CreatedAt must be a valid date.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "CreatedAt"
                    }
                );

            _createdAt = value;
        }
    }

    public Guid CreatedById
    {
        get => _createdById;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    EntityErrorCodes.EmptyCreatedById,
                    "CreatedById cannot be empty.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "CreatedById"
                    }
                );

            _createdById = value;
        }
    }

    public DateTimeOffset? UpdatedAt
    {
        get => _updatedAt;
        set
        {
            if (value.HasValue && value.Value == default)
                throw new ValidationException(
                    EntityErrorCodes.InvalidUpdatedAt,
                    "UpdatedAt must be a valid date.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "UpdatedAt"
                    }
                );

            _updatedAt = value;
        }
    }

    public Guid? UpdatedById
    {
        get => _updatedById;
        set
        {
            if (value.HasValue && value.Value == Guid.Empty)
                throw new ValidationException(
                    EntityErrorCodes.EmptyUpdatedById,
                    "UpdatedById cannot be empty.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "UpdatedById"
                    }
                );

            _updatedById = value;
        }
    }

    public bool IsDeleted
    {
        get => _isDeleted;
        set => _isDeleted = value;
    }

    public DateTimeOffset? DeletedAt
    {
        get => _deletedAt;
        set
        {
            if (value.HasValue && value.Value == default)
                throw new ValidationException(
                    EntityErrorCodes.InvalidDeletedAt,
                    "DeletedAt must be a valid date.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "DeletedAt"
                    }
                );

            _deletedAt = value;
        }
    }

    public Guid? DeletedById
    {
        get => _deletedById;
        set
        {
            if (value.HasValue && value.Value == Guid.Empty)
                throw new ValidationException(
                    EntityErrorCodes.EmptyDeletedById,
                    "DeletedById cannot be empty.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "DeletedById"
                    }
                );

            _deletedById = value;
        }
    }

    public void SoftDelete(Guid deletedById)
    {
        if (IsDeleted)
            throw new ValidationException(
                EntityErrorCodes.HasBeenDeleted,
                "Entity has already been deleted."
            );

        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedById = deletedById;
    }

    public void CreateAudit(Guid createdById)
    {
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedById = createdById;
    }

    public void UpdateAudit(Guid updatedById)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedById = updatedById;
    }

    public User? CreatedBy { get; set; }
    public User? UpdatedBy { get; set; }
    public User? DeletedBy { get; set; }
}