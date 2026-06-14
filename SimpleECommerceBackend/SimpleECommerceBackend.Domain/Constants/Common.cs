namespace SimpleECommerceBackend.Domain.Constants;

public static class Common
{
    private const string SystemUserId = "00000000-0000-0000-0000-000000000001";

    public static Guid GetSystemUserId()
    {
        return Guid.Parse(SystemUserId);
    }
}