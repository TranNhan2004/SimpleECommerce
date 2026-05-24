using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductVariantImage : EntityBase, ICreatedTrackable
{
    public ProductVariantImage()
    {
    }

    private Guid _productVariantId;
    private string _imageUrl = null!;
    private int _displayOrder;
    private string? _description;

    public Guid ProductVariantId
    {
        get => _productVariantId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ProductVariantImageErrorCodes.ProductVariantIdRequired,
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

    public string ImageUrl
    {
        get => _imageUrl;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ProductVariantImageErrorCodes.ImageUrlRequired,
                    "Image URL is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ImageUrl"
                    }
                );

            _imageUrl = value.Trim();
        }
    }

    public int DisplayOrder
    {
        get => _displayOrder;
        set
        {
            if (value < 0)
                throw new ValidationException(
                    ProductVariantImageErrorCodes.DisplayOrderCannotBeNegative,
                    "Display order cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "DisplayOrder"
                    }
                );

            _displayOrder = value;
        }
    }

    public bool IsDisplayed { get; set; }

    public string? Description
    {
        get => _description;
        set
        {
            if (value is null)
            {
                _description = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ProductVariantImageErrorCodes.DescriptionMustNotBeBlank,
                    "Description must not be blank",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description"
                    }
                );

            var trimmedDescription = value.Trim();
            if (trimmedDescription.Length > ProductVariantImageValidationRules.DescriptionMaxLength)
                throw new ValidationException(
                    ProductVariantImageErrorCodes.DescriptionMaxLengthExceeded,
                    $"Description cannot exceed {ProductVariantImageValidationRules.DescriptionMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description",
                        ["max"] = ProductVariantImageValidationRules.DescriptionMaxLength
                    }
                );

            _description = trimmedDescription;
        }
    }

    public DateTimeOffset CreatedAt { get; private set; }
}
