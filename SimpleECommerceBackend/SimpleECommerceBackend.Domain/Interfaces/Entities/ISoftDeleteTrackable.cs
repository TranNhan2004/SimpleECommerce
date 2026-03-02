namespace SimpleECommerceBackend.Domain.Interfaces.Entities;

public interface ISoftDeleteTrackable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }

    void SoftDelete();
}