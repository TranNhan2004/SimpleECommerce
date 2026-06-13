using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductVariantPrice : EntityBase
{
    public ProductVariantPrice()
    {
    }

    private Guid _productVariantId;
    private Money _money;
    private DateTimeOffset _effectiveFrom;

    public Guid ProductVariantId
    {
        get => _productVariantId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ProductVariantPriceErrorCodes.ProductVariantIdRequired,
                    "Product variant is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ProductVariantId"
                    }
                );

            _productVariantId = value;
        }
    }

    public ProductVariant? ProductVariant { get; private set; }

    public Money Money
    {
        get => _money;
        set
        {
            if (value.Amount <= 0)
                throw new ValidationException(
                    ProductVariantPriceErrorCodes.PriceMustBePositive,
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
}
