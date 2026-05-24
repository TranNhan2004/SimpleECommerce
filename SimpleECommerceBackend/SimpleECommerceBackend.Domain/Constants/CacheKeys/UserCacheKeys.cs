namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class UserCacheKeys
{
    public const string GetProfile = "User:{id}";
    public const int GetProfileTtlMinutes = 30;

    public static string GetProfileKey(Guid id)
    {
        return GetProfile.Replace("{id}", id.ToString());
    }
}
