namespace SimpleECommerceBackend.Api.DTOs.Auth;

public class RefreshTokenResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
}
