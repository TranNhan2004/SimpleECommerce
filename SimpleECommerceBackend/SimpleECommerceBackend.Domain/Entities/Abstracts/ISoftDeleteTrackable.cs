namespace SimpleECommerceBackend.Domain.Entities.Abstracts;

public interface ISoftDeleteTrackable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }

    void SoftDelete();
}