namespace SimpleECommerceBackend.Application.Models.Auth;

public class RefreshTokenCommand
{
    public string SessionId { get; init; } = null!;
    public string? CsrfCookieToken { get; init; }
    public string? CsrfHeaderToken { get; init; }
}
