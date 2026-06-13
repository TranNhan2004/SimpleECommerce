using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Product : EntityBase
{
    private readonly List<ProductVariant> _productVariants = [];
    private readonly List<Review> _reviews = [];

    public Product()
    {
    }

    private string _name = null!;
    private string _description = null!;
    private Guid _categoryId;
    private Guid _sellerId;
    private double _averageRating;
    private int _totalRatings;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ProductErrorCodes.NameRequired,
                    "Name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Name"
                    }
                );

            var trimmedName = value.Trim();
            if (trimmedName.Length > ProductValidationRules.NameMaxLength)
                throw new ValidationException(
                    ProductErrorCodes.NameMaxLengthExceeded,
                    $"Name cannot exceed {ProductValidationRules.NameMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Name",
                        ["max"] = ProductValidationRules.NameMaxLength
                    }
                );

            _name = trimmedName;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ProductErrorCodes.DescriptionRequired,
                    "Description is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description"
                    }
                );

            var trimmedDescription = value.Trim();
            if (trimmedDescription.Length > ProductValidationRules.DescriptionMaxLength)
                throw new ValidationException(
                    ProductErrorCodes.DescriptionMaxLengthExceeded,
                    $"Description cannot exceed {ProductValidationRules.DescriptionMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description",
                        ["max"] = ProductValidationRules.DescriptionMaxLength
                    }
                );

            _description = trimmedDescription;
        }
    }

    public Guid CategoryId
    {
        get => _categoryId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ProductErrorCodes.CategoryRequired,
                    "Category is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Category"
                    }
                );

            _categoryId = value;
        }
    }

    public Category? Category { get; set; }

    public Guid SellerId
    {
        get => _sellerId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ProductErrorCodes.SellerRequired,
                    "Seller is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Seller"
                    }
                );

            _sellerId = value;
        }
    }

    public User? Seller { get; set; }

    public double AverageRating
    {
        get => _averageRating;
        set
        {
            if (value < 0 || value > 5)
                throw new ValidationException(
                    ProductErrorCodes.AverageRatingOutOfRange,
                    "Average rating must be between 0 and 5",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "AverageRating",
                        ["min"] = 0,
                        ["max"] = 5
                    }
                );

            _averageRating = value;
        }
    }

    public int TotalRatings
    {
        get => _totalRatings;
        set
        {
            if (value < 0)
                throw new ValidationException(
                    ProductErrorCodes.TotalRatingsCannotBeNegative,
                    "Total ratings cannot be negative",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "TotalRatings"
                    }
                );

            _totalRatings = value;
        }
    }

    public ProductStatus Status { get; set; } = ProductStatus.Active;

    public IReadOnlyCollection<ProductVariant> ProductVariants => _productVariants;
    public IReadOnlyCollection<Review> Reviews => _reviews;
    public void AddVariant(ProductVariant variant)
    {
        _productVariants.Add(variant);
    }

    public void AddReview(Review review)
    {
        _reviews.Add(review);
    }
}
