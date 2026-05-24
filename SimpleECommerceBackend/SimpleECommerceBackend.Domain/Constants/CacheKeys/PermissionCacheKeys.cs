namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class PermissionCacheKeys
{
    public const string PermissionSet = "PermissionSet:{userId}";
    public const string PermissionSetPrefix = "PermissionSet";
    public const int PermissionSetTtlMinutes = 15;

    public static string GetPermissionSetKey(Guid userId)
    {
        return PermissionSet.Replace("{userId}", userId.ToString());
    }
}
