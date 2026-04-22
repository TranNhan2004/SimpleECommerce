namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class ProductImageErrorCodes
{
    public const string ImageUrlRequired = "ProductImage_ImageUrlRequired";
    public const string DisplayOrderCannotBeNegative = "ProductImage_DisplayOrderCannotBeNegative";
    public const string DescriptionMustNotBeBlank = "ProductImage_DescriptionMustNotBeBlank";
    public const string DescriptionMaxLengthExceeded = "ProductImage_DescriptionMaxLengthExceeded";
}
