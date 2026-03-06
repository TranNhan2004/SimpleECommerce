namespace SimpleECommerceBackend.Domain.Entities.Abstracts;

public interface ICreatedTrackable
{
    DateTimeOffset CreatedAt { get; }
}