using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductImage : Entity, ICreatedTrackable
{
    private ProductImage()
    {
    }

    private ProductImage(
        string imageUrl,
        int displayOrder,
        bool isDisplayed,
        string? description
    )
    {
        SetId(Guid.NewGuid());
        SetImageUrl(imageUrl);
        SetDisplayOrder(displayOrder);
        SetIsDisplayed(isDisplayed);
        SetDescription(description);
    }

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string ImageUrl { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public bool IsDisplayed { get; private set; }
    public string? Description { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static ProductImage Create(
        string imageUrl,
        int displayOrder,
        bool isDisplayed = true,
        string? description = null
    )
    {
        return new ProductImage(
            imageUrl,
            displayOrder,
            isDisplayed,
            description
        );
    }

    public void SetImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ValidationException(
                ProductImageErrorCode.ImageUrlRequired,
                "Image URL is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "ImageUrl"
                }
            );

        ImageUrl = imageUrl.Trim();
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new ValidationException(
                ProductImageErrorCode.DisplayOrderCannotBeNegative,
                "Display order cannot be negative",
                new Dictionary<string, object?>
                {
                    ["field"] = "DisplayOrder"
                }
            );

        DisplayOrder = displayOrder;
    }

    public void SetIsDisplayed(bool isDisplayed)
    {
        IsDisplayed = isDisplayed;
    }

    public void SetDescription(string? description)
    {
        if (description is null)
        {
            Description = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(description))
            throw new ValidationException(
                ProductImageErrorCode.DescriptionMustNotBeBlank,
                "Description is not blank",
                new Dictionary<string, object?>
                {
                    ["field"] = "Description"
                }
            );

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > ProductImageConstants.DescriptionMaxLength)
            throw new ValidationException(
                ProductImageErrorCode.DescriptionMaxLengthExceeded,
                $"Description cannot exceed {ProductImageConstants.DescriptionMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Description",
                    ["max"] = ProductImageConstants.DescriptionMaxLength
                }
            );

        Description = trimmedDescription;
    }
}
