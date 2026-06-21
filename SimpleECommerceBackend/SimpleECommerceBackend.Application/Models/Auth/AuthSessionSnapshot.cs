namespace SimpleECommerceBackend.Application.Models.Auth;

public class AuthSessionSnapshot
{
    public string SessionId { get; init; } = null!;
    public string Subject { get; init; } = null!;
    public string Email { get; init; } = string.Empty;
    public string PreferredUsername { get; init; } = string.Empty;
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public string? IdToken { get; init; }
    public string CsrfToken { get; init; } = null!;
    public DateTimeOffset SessionExpiresAtUtc { get; init; }
    public DateTimeOffset AccessTokenExpiresAtUtc { get; init; }
    public DateTimeOffset RefreshTokenExpiresAtUtc { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset LastRefreshedAtUtc { get; init; }
}
