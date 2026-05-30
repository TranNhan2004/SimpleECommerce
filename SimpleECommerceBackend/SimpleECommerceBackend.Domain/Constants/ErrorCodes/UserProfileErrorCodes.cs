namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class UserProfileErrorCodes
{
    public const string KeycloakSubjectIdInvalid = "UserProfile_KeycloakSubjectIdInvalid";
    public const string FirstNameRequired = "UserProfile_FirstNameRequired";
    public const string FirstNameMaxLengthExceeded = "UserProfile_FirstNameMaxLengthExceeded";
    public const string LastNameRequired = "UserProfile_LastNameRequired";
    public const string LastNameMaxLengthExceeded = "UserProfile_LastNameMaxLengthExceeded";
    public const string NickNameMustNotBeBlank = "UserProfile_NickNameMustNotBeBlank";
    public const string NickNameMaxLengthExceeded = "UserProfile_NickNameMaxLengthExceeded";
    public const string BirthDateCannotBeFuture = "UserProfile_BirthDateCannotBeFuture";
    public const string AgeCannotBeLessThan = "UserProfile_AgeCannotBeLessThan";
    public const string AgeCannotExceed = "UserProfile_AgeCannotExceed";
    public const string ArchiveNotAllowed = "UserProfile_ArchiveNotAllowed";
    public const string ActivateNotAllowed = "UserProfile_ActivateNotAllowed";
    public const string InactiveUser = "UserProfile_InactiveUser";
    public const string NotFoundById = "UserProfile_NotFoundById";
    public const string NotFoundByKeycloakSubjectId = "UserProfile_NotFoundByKeycloakSubjectId";
}
