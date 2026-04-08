namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class CurrentUserErrorCode
{
    public const string Unauthenticated = "CurrentUser_Unauthenticated";
    public const string UserIdMissing = "CurrentUser_UserIdMissing";
    public const string UserIdInvalid = "CurrentUser_UserIdInvalid";
    public const string RoleMissing = "CurrentUser_RoleMissing";
}
