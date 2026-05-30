namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class CurrentUserErrorCodes
{
    public const string Unauthenticated = "CurrentUser_Unauthenticated";
    public const string SubjectIdMissing = "CurrentUser_SubjectIdMissing";
    public const string SubjectIdInvalid = "CurrentUser_SubjectIdInvalid";
}
