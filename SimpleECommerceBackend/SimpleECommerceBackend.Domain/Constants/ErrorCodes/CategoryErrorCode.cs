namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class CategoryErrorCode
{
    public const string NameRequired = "Category_NameRequired";
    public const string NameMaxLengthExceeded = "Category_NameMaxLengthExceeded";
    public const string DescriptionMustNotBeBlank = "Category_DescriptionMustNotBeBlank";
    public const string DescriptionMaxLengthExceeded = "Category_DescriptionMaxLengthExceeded";
    public const string ActivateNotAllowed = "Category_ActivateNotAllowed";
    public const string DeactivateNotAllowed = "Category_DeactivateNotAllowed";
    public const string AlreadyArchived = "Category_AlreadyArchived";
    public const string AdminRequired = "Category_AdminRequired";
}
