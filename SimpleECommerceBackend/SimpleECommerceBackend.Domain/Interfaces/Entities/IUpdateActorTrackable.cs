namespace SimpleECommerceBackend.Domain.Interfaces;

public interface IUpdatedByUser
{
    Guid? UpdatedBy { get; }
}