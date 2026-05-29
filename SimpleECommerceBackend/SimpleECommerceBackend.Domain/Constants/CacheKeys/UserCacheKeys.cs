namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class UserCacheKeys
{
    public const string GetUserByKeycloakSubjectId = "User:{keycloakSubjectId}";
    public const int GetUserByKeycloakSubjectIdTtlMinutes = 864_000;

    public const string GetUserById = "User:{id}";
    public const int GetUserByIdTtlMinutes = 30;

    public static string GetUserByKeycloakSubjectIdKey(Guid keycloakSubjectId)
    {
        return GetUserByKeycloakSubjectId.Replace("{keycloakSubjectId}", keycloakSubjectId.ToString());
    }

    public static string GetUserByIdKey(Guid id)
    {
        return GetUserById.Replace("{id}", id.ToString());
    }
}
