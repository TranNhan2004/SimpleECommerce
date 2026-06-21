using SimpleECommerceBackend.Application.Models.Auth;

namespace SimpleECommerceBackend.Api.Dtos.V1.Auth;

public class RefreshTokenResponse
{
    public DateTimeOffset SessionExpiresAtUtc { get; init; }
    public DateTimeOffset AccessTokenExpiresAtUtc { get; init; }
    public DateTimeOffset RefreshTokenExpiresAtUtc { get; init; }

    public static RefreshTokenResponse FromResult(RefreshTokenResult result)
    {
        return new RefreshTokenResponse
        {
            SessionExpiresAtUtc = result.SessionExpiresAtUtc,
            AccessTokenExpiresAtUtc = result.AccessTokenExpiresAtUtc,
            RefreshTokenExpiresAtUtc = result.RefreshTokenExpiresAtUtc
        };
    }
}
