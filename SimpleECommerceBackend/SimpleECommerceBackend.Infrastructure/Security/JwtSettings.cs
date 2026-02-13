namespace SimpleECommerceBackend.Infrastructure.Security;

public class JwtSettings
{
    public string Secret { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int AccessTokenExpirationMinutes { get; init; }
    public int RefreshTokenExpirationMinutes { get; init; }
    public int AccountVerificationTokenExpirationMinutes { get; init; }
    public int PasswordResetTokenExpirationMinutes { get; init; }
}