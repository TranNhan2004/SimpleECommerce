using SimpleECommerceBackend.Domain.Interfaces;

namespace SimpleECommerceBackend.Domain.Entities;

public class EntityBase : IEntity
{
    public Guid Id { get; private set; }
}