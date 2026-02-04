using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities;

public class EntityBase : IEntity
{
    public Guid Id { get; private set; }
}