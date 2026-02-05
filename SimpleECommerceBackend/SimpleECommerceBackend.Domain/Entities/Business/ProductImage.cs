using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ProductImage
{
    private ProductImage()
    {
    }

    private ProductImage(
        Guid productId,
        string imageUrl,
        int displayOrder,
        bool isDisplayed,
        string? description
    )
    {
        SetProductId(productId);
        SetImageUrl(imageUrl);
        SetDisplayOrder(displayOrder);
        SetIsDisplayed(isDisplayed);
        SetDescription(description);
    }

    public Guid ProductId { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsDisplayed { get; private set; }
    public string? Description { get; private set; }

    public static ProductImage Create(
        Guid productId,
        string imageUrl,
        int displayOrder,
        bool isDisplayed = true,
        string? description = null
    )
    {
        return new ProductImage(
            productId,
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

        Description = description.Trim();
    }

    private void SetProductId(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product ID is required");

        ProductId = productId;
    }
}