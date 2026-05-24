namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class ProductVariantImageErrorCodes
{
    public const string ProductVariantIdRequired = "ProductVariantImage_ProductVariantIdRequired";
    public const string ImageUrlRequired = "ProductVariantImage_ImageUrlRequired";
    public const string DisplayOrderCannotBeNegative = "ProductVariantImage_DisplayOrderCannotBeNegative";
    public const string DescriptionMustNotBeBlank = "ProductVariantImage_DescriptionMustNotBeBlank";
    public const string DescriptionMaxLengthExceeded = "ProductVariantImage_DescriptionMaxLengthExceeded";
}
