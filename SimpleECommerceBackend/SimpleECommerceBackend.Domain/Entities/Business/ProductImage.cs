using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductImage : Entity, ICreatedTrackable
{
    public ProductImage()
    {
    }

    private ProductImage(
        string imageUrl,
        int displayOrder,
        bool isDisplayed,
        string? description
    )
    {
        Id = Guid.NewGuid();
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        IsDisplayed = isDisplayed;
        Description = description;
    }

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    private string _imageUrl = null!;
    private int _displayOrder;
    private bool _isDisplayed;
    private string? _description;

    public string ImageUrl
    {
        get => _imageUrl;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ProductImageErrorCodes.ImageUrlRequired,
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
                    ProductImageErrorCodes.DisplayOrderCannotBeNegative,
                    "Display order cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "DisplayOrder"
                    }
                );

            _displayOrder = value;
        }
    }

    public bool IsDisplayed
    {
        get => _isDisplayed;
        set => _isDisplayed = value;
    }

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
                    ProductImageErrorCodes.DescriptionMustNotBeBlank,
                    "Description is not blank",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description"
                    }
                );

            var trimmedDescription = value.Trim();

            if (trimmedDescription.Length > ProductImageValidationRules.DescriptionMaxLength)
                throw new ValidationException(
                    ProductImageErrorCodes.DescriptionMaxLengthExceeded,
                    $"Description cannot exceed {ProductImageValidationRules.DescriptionMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description",
                        ["max"] = ProductImageValidationRules.DescriptionMaxLength
                    }
                );

            _description = trimmedDescription;
        }
    }

    public DateTimeOffset CreatedAt { get; private set; }
}
