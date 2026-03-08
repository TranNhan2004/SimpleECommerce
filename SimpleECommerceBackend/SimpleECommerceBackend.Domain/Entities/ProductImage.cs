using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities;

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
            throw new BusinessException("Image URL is required");

        ImageUrl = imageUrl.Trim();
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new BusinessException("Display order cannot be negative");

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
            throw new BusinessException("Description is not blank");

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > ProductImageConstants.DescriptionMaxLength)
            throw new BusinessException(
                $"Description cannot exceed {CategoryConstants.DescriptionMaxLength} characters");

        Description = trimmedDescription;
    }
}