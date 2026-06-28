namespace SimpleECommerceBackend.Domain.Entities;

public interface ISoftDeleteTrackable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    Guid? DeletedById { get; }

    void SoftDelete(Guid deletedById);
}