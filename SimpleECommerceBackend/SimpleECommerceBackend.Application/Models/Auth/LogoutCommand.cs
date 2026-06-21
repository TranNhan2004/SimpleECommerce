namespace SimpleECommerceBackend.Application.Models.Auth;

public class LogoutCommand
{
    public string? SessionId { get; init; }
    public string? CsrfCookieToken { get; init; }
    public string? CsrfHeaderToken { get; init; }
}
