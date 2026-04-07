namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class UserProfileErrorCode
{
    public const string FirstNameRequired = "UserProfile_FirstNameRequired";
    public const string FirstNameMaxLengthExceeded = "UserProfile_FirstNameMaxLengthExceeded";
    public const string LastNameRequired = "UserProfile_LastNameRequired";
    public const string LastNameMaxLengthExceeded = "UserProfile_LastNameMaxLengthExceeded";
    public const string NickNameMustNotBeBlank = "UserProfile_NickNameMustNotBeBlank";
    public const string NickNameMaxLengthExceeded = "UserProfile_NickNameMaxLengthExceeded";
    public const string BirthDateCannotBeFuture = "UserProfile_BirthDateCannotBeFuture";
    public const string AgeCannotBeLessThan = "UserProfile_AgeCannotBeLessThan";
    public const string AgeCannotExceed = "UserProfile_AgeCannotExceed";
}
