using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Abstracts;

public class Entity : IEntity
{
    public Guid Id { get; private set; }

    protected virtual void SetId(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException(
                EntityErrorCodes.EmptyId,
                "Id cannot be empty.",
                new Dictionary<string, object?>
                {
                    ["field"] = "Id"
                }
            );

        Id = id;
    }
}