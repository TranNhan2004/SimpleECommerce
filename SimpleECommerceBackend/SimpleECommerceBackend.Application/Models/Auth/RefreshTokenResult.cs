namespace SimpleECommerceBackend.Application.Models.Auth;

public class RefreshTokenResult
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
}
