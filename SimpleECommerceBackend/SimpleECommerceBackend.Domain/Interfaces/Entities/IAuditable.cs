namespace SimpleECommerceBackend.Domain.Interfaces.Entities;

public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
}