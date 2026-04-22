namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class UserProfileCacheKeys
{
    public const string GetProfile = "UserProfile:{id}";
    public const int GetProfileTtlMinutes = 30;
}