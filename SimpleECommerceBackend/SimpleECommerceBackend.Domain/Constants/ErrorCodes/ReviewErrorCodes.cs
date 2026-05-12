namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class ReviewErrorCodes
{
    public const string ProductIdRequired = "Review_ProductIdRequired";
    public const string CustomerIdRequired = "Review_CustomerIdRequired";
    public const string RatingOutOfRange = "Review_RatingOutOfRange";
    public const string CommentRequired = "Review_CommentRequired";
    public const string CommentMaxLengthExceeded = "Review_CommentMaxLengthExceeded";
}
