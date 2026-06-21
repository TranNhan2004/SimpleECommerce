namespace SimpleECommerceBackend.Application.Models.Auth;

public class RefreshTokenResult
{
    public DateTimeOffset SessionExpiresAtUtc { get; init; }
    public DateTimeOffset AccessTokenExpiresAtUtc { get; init; }
    public DateTimeOffset RefreshTokenExpiresAtUtc { get; init; }
}
