namespace SimpleECommerceBackend.Domain.Constants.ErrorCodes;

public static class AuthErrorCodes
{
    public const string InvalidReturnUrl = "Auth_InvalidReturnUrl";
    public const string OAuthStateMissing = "Auth_OAuthStateMissing";
    public const string OAuthStateInvalid = "Auth_OAuthStateInvalid";
    public const string OAuthCodeMissing = "Auth_OAuthCodeMissing";
    public const string OAuthProviderError = "Auth_OAuthProviderError";
    public const string TokenExchangeFailed = "Auth_TokenExchangeFailed";
    public const string UserInfoFailed = "Auth_UserInfoFailed";
    public const string SessionNotFound = "Auth_SessionNotFound";
    public const string RefreshTokenMissing = "Auth_RefreshTokenMissing";
    public const string CsrfTokenMissing = "Auth_CsrfTokenMissing";
    public const string CsrfTokenInvalid = "Auth_CsrfTokenInvalid";
}
