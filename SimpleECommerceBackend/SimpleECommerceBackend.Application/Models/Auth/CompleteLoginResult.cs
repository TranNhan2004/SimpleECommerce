namespace SimpleECommerceBackend.Application.Models.Auth;

public class CompleteLoginResult
{
    public string SessionId { get; init; } = null!;
    public string CsrfToken { get; init; } = null!;
    public string FrontendRedirectUrl { get; init; } = null!;
    public DateTimeOffset SessionExpiresAtUtc { get; init; }
    public DateTimeOffset AccessTokenExpiresAtUtc { get; init; }
    public DateTimeOffset RefreshTokenExpiresAtUtc { get; init; }
}
