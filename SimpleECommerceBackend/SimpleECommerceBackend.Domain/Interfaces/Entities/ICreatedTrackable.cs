namespace SimpleECommerceBackend.Domain.Interfaces.Entities;

public interface ICreatedTrackable
{
    DateTimeOffset CreatedAt { get; }
}