namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class UserProfileCacheKeys
{
    public const string GetProfile = "UserProfile:{id}";
    public const int GetProfileTtlMinutes = 30;

    public static string GetProfileKey(Guid id)
    {
        return GetProfile.Replace("{id}", id.ToString());
    }
}