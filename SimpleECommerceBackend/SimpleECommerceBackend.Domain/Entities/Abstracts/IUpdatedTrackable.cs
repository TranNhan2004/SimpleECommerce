namespace SimpleECommerceBackend.Domain.Entities.Abstracts;

public interface IUpdatedTrackable
{
    DateTimeOffset? UpdatedAt { get; }
}