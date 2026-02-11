using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductImage : EntityBase, ICreatedTime
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
        SetImageUrl(imageUrl);
        SetDisplayOrder(displayOrder);
        SetIsDisplayed(isDisplayed);
        SetDescription(description);
    }

    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
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
            throw new DomainException("Image URL is required");

        ImageUrl = imageUrl.Trim();
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new DomainException("Display order cannot be negative");

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
            throw new DomainException("Description is not blank");

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > ProductImageConstants.DescriptionMaxLength)
            throw new DomainException(
                $"Description cannot exceed {CategoryConstants.DescriptionMaxLength} characters");

        Description = trimmedDescription;
    }
}