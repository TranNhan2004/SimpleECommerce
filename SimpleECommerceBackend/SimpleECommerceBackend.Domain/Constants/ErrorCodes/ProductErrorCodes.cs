namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class ProductErrorCodes
{
    public const string NotFoundById = "Product_NotFoundById";
    public const string NameRequired = "Product_NameRequired";
    public const string NameMaxLengthExceeded = "Product_NameMaxLengthExceeded";
    public const string DescriptionRequired = "Product_DescriptionRequired";
    public const string DescriptionMaxLengthExceeded = "Product_DescriptionMaxLengthExceeded";
    public const string ActivateNotAllowed = "Product_ActivateNotAllowed";
    public const string HideNotAllowed = "Product_HideNotAllowed";
    public const string CategoryRequired = "Product_CategoryRequired";
    public const string SellerRequired = "Product_SellerRequired";
    public const string ImageNotFound = "Product_ImageNotFound";
    public const string AlreadyDeleted = "Product_AlreadyDeleted";
    public const string TotalInStockCannotBeNegative = "Product_TotalInStockCannotBeNegative";
    public const string AverageRatingOutOfRange = "Product_AverageRatingOutOfRange";
    public const string TotalRatingsCannotBeNegative = "Product_TotalRatingsCannotBeNegative";
}
