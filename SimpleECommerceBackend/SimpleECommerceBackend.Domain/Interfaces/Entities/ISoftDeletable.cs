namespace SimpleECommerceBackend.Domain.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
}