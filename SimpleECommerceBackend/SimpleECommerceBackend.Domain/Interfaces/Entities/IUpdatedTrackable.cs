namespace SimpleECommerceBackend.Domain.Interfaces.Entities;

public interface IUpdatedTrackable
{
    DateTimeOffset? UpdatedAt { get; }
}