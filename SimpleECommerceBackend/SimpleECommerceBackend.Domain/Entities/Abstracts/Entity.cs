using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Abstracts;

public class Entity : IEntity
{
    public Guid Id { get; protected set; }

    protected virtual void SetId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("Id cannot be empty.");
        }

        Id = id;
    }
}