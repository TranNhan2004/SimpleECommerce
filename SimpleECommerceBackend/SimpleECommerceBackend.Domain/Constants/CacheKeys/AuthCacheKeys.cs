namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class AuthCacheKeys
{
    public const string OAuthStatePrefix = "auth:state:";
    public const string SessionPrefix = "auth:";

    public static string GetOAuthStateKey(string state)
    {
        return $"{OAuthStatePrefix}{state}";
    }

    public static string GetSessionKey(string sessionId)
    {
        return $"{SessionPrefix}{sessionId}";
    }
}
