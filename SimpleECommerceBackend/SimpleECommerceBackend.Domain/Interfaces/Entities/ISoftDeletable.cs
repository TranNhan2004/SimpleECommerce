namespace SimpleECommerceBackend.Domain.Interfaces.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }

    void SoftDelete();
}