namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class UserErrorCodes
{
    public const string KeycloakSubjectIdInvalid = "User_KeycloakSubjectIdInvalid";
    public const string FirstNameRequired = "User_FirstNameRequired";
    public const string FirstNameMaxLengthExceeded = "User_FirstNameMaxLengthExceeded";
    public const string LastNameRequired = "User_LastNameRequired";
    public const string LastNameMaxLengthExceeded = "User_LastNameMaxLengthExceeded";
    public const string NickNameMustNotBeBlank = "User_NickNameMustNotBeBlank";
    public const string NickNameMaxLengthExceeded = "User_NickNameMaxLengthExceeded";
    public const string BirthDateCannotBeFuture = "User_BirthDateCannotBeFuture";
    public const string AgeCannotBeLessThan = "User_AgeCannotBeLessThan";
    public const string AgeCannotExceed = "User_AgeCannotExceed";
    public const string ArchiveNotAllowed = "User_ArchiveNotAllowed";
    public const string ActivateNotAllowed = "User_ActivateNotAllowed";
    public const string InactiveUser = "User_InactiveUser";
    public const string NotFoundById = "User_NotFoundById";
    public const string NotFoundByKeycloakSubjectId = "User_NotFoundByKeycloakSubjectId";
}
