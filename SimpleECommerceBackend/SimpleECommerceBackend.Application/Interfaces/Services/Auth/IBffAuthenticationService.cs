using SimpleECommerceBackend.Application.Models.Auth;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Auth;

public interface IBffAuthenticationService
{
    Task<BeginLoginResult> BeginLoginAsync(string? returnUrl, CancellationToken cancellationToken = default);
    Task<CompleteLoginResult> CompleteLoginAsync(CompleteLoginCommand command, CancellationToken cancellationToken = default);
    Task<LogoutResult> LogoutAsync(string? sessionId, string? csrfCookieToken, string? csrfHeaderToken, CancellationToken cancellationToken = default);
    Task<RefreshTokenResult> RefreshTokenAsync(string sessionId, string? csrfCookieToken, string? csrfHeaderToken, CancellationToken cancellationToken = default);
    Task<AuthSessionSnapshot?> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}
