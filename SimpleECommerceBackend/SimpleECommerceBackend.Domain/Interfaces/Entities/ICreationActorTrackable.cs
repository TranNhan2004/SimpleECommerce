namespace SimpleECommerceBackend.Domain.Interfaces.Entities;

public interface ICreationActorTrackable
{
    Guid CreatedBy { get; }
    void MarkCreatedBy(Guid actorId);
}