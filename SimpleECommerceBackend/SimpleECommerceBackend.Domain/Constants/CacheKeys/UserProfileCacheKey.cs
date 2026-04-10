namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class UserProfileCacheKey
{
    public const string GetProfile = "UserProfile:GetProfile:{id}";
    public const int GetProfileTtlMinutes = 30;
}