namespace SimpleECommerceBackend.Domain.Utils;

public static class UuidUtils
{
    public static Guid CreateV7()
    {
        return Guid.CreateVersion7(DateTimeOffset.UtcNow);
    }
}