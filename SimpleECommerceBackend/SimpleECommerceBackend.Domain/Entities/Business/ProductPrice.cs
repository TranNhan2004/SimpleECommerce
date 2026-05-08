using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductPrice : Entity, ICreatedTrackable
{
    public ProductPrice()
    {
    }

    private ProductPrice(Guid productId, Money money, DateTimeOffset effectiveFrom)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Money = money;
        EffectiveFrom = effectiveFrom;
    }

    private Guid _productId;
    private Money _money;
    private DateTimeOffset _effectiveFrom;

    public Guid ProductId
    {
        get => _productId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ProductPriceErrorCodes.ProductIdRequired,
                    "ProductId is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ProductId"
                    }
                );

            _productId = value;
        }
    }

    public Product? Product { get; private set; }
    public Money Money
    {
        get => _money;
        set
        {
            if (value.Amount <= 0)
                throw new ValidationException(
                    ProductPriceErrorCodes.PriceMustBePositive,
                    "Price amount must be positive",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Price"
                    }
                );

            _money = value;
        }
    }

    public DateTimeOffset EffectiveFrom
    {
        get => _effectiveFrom;
        set => _effectiveFrom = value;
    }

    public DateTimeOffset CreatedAt { get; private set; }

}
