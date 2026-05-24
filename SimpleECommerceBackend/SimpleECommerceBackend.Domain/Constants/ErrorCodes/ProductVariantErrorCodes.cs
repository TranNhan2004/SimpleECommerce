namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class ProductVariantErrorCodes
{
    public const string ProductRequired = "ProductVariant_ProductRequired";
    public const string NameRequired = "ProductVariant_NameRequired";
    public const string NameMaxLengthExceeded = "ProductVariant_NameMaxLengthExceeded";
    public const string DescriptionRequired = "ProductVariant_DescriptionRequired";
    public const string DescriptionMaxLengthExceeded = "ProductVariant_DescriptionMaxLengthExceeded";
    public const string CurrentPriceMustBePositive = "ProductVariant_CurrentPriceMustBePositive";
    public const string TotalInStockCannotBeNegative = "ProductVariant_TotalInStockCannotBeNegative";
    public const string ImageNotFound = "ProductVariant_ImageNotFound";
}
