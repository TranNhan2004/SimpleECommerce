namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class ReviewResponseErrorCodes
{
    public const string ReviewIdRequired = "ReviewResponse_ReviewIdRequired";
    public const string FromUserIdRequired = "ReviewResponse_FromUserIdRequired";
    public const string ToUserIdRequired = "ReviewResponse_ToUserIdRequired";
    public const string CommentRequired = "ReviewResponse_CommentRequired";
    public const string CommentMaxLengthExceeded = "ReviewResponse_CommentMaxLengthExceeded";
}
