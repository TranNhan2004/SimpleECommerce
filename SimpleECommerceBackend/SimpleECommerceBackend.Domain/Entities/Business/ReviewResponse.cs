using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class ReviewResponse : EntityBase
{
    public ReviewResponse()
    {
    }

    private Guid _reviewId;
    private Guid _fromUserId;
    private Guid _toUserId;
    private string _comment = null!;

    public Guid ReviewId
    {
        get => _reviewId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ReviewResponseErrorCodes.ReviewIdRequired,
                    "Review is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ReviewId"
                    }
                );

            _reviewId = value;
        }
    }

    public Review? Review { get; private set; }

    public Guid FromUserId
    {
        get => _fromUserId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ReviewResponseErrorCodes.FromUserIdRequired,
                    "From user is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "FromUserId"
                    }
                );

            _fromUserId = value;
        }
    }

    public User? FromUser { get; private set; }

    public Guid ToUserId
    {
        get => _toUserId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    ReviewResponseErrorCodes.ToUserIdRequired,
                    "To user is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ToUserId"
                    }
                );

            _toUserId = value;
        }
    }

    public User? ToUser { get; private set; }

    public string Comment
    {
        get => _comment;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    ReviewResponseErrorCodes.CommentRequired,
                    "Comment is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Comment"
                    }
                );

            var trimmedComment = value.Trim();
            if (trimmedComment.Length > ReviewResponseValidationRules.CommentMaxLength)
                throw new ValidationException(
                    ReviewResponseErrorCodes.CommentMaxLengthExceeded,
                    $"Comment cannot exceed {ReviewResponseValidationRules.CommentMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Comment",
                        ["max"] = ReviewResponseValidationRules.CommentMaxLength
                    }
                );

            _comment = trimmedComment;
        }
    }
}
