namespace SimpleECommerceBackend.Domain.Interfaces.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }

    void SoftDelete();
}