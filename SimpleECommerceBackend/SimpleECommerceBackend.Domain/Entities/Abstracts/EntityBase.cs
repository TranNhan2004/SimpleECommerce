using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Abstracts;

public class EntityBase : IEntity
{
    private Guid _id;

    public Guid Id
    {
        get => _id;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    EntityErrorCodes.EmptyId,
                    "Id cannot be empty.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Id"
                    }
                );

            _id = value;
        }
    }
}
