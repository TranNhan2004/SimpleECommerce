namespace SimpleECommerceBackend.Domain.Interfaces.Entities;

public interface IUpdateActorTrackable
{
    Guid? UpdatedBy { get; }
    void MarkUpdatedBy(Guid actorId);
}