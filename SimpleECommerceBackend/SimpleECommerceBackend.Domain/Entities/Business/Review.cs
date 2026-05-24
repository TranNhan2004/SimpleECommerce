using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Review : EntityBase, ICreatedTrackable, IUpdatedTrackable
{
    private readonly List<ReviewResponse> _responses = [];

    public Review()
    {
    }

    private Guid _productId;
    private Guid _customerId;
    private int _rating;
    private string _comment = null!;

    public Guid ProductId
    {
        get => _productId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ReviewErrorCodes.ProductIdRequired,
                    "Product is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ProductId"
                    }
                );

            _productId = value;
        }
    }

    public Product? Product { get; private set; }

    public Guid CustomerId
    {
        get => _customerId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ReviewErrorCodes.CustomerIdRequired,
                    "Customer is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "CustomerId"
                    }
                );

            _customerId = value;
        }
    }

    public User? Customer { get; private set; }

    public int Rating
    {
        get => _rating;
        set
        {
            if (value < ReviewValidationRules.MinRating || value > ReviewValidationRules.MaxRating)
                throw new ValidationException(
                    ReviewErrorCodes.RatingOutOfRange,
                    $"Rating must be between {ReviewValidationRules.MinRating} and {ReviewValidationRules.MaxRating}",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Rating",
                        ["min"] = ReviewValidationRules.MinRating,
                        ["max"] = ReviewValidationRules.MaxRating
                    }
                );

            _rating = value;
        }
    }

    public string Comment
    {
        get => _comment;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ReviewErrorCodes.CommentRequired,
                    "Comment is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Comment"
                    }
                );

            var trimmedComment = value.Trim();
            if (trimmedComment.Length > ReviewValidationRules.CommentMaxLength)
                throw new ValidationException(
                    ReviewErrorCodes.CommentMaxLengthExceeded,
                    $"Comment cannot exceed {ReviewValidationRules.CommentMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Comment",
                        ["max"] = ReviewValidationRules.CommentMaxLength
                    }
                );

            _comment = trimmedComment;
        }
    }

    public IReadOnlyCollection<ReviewResponse> Responses => _responses;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public void AddResponse(ReviewResponse response)
    {
        _responses.Add(response);
    }
}
