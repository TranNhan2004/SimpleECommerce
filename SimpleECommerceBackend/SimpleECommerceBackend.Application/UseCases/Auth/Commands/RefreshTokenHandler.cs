using SimpleECommerceBackend.Application.Interfaces.Services.Auth;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Commands;

public class RefreshTokenHandler : IUseCaseHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IBffAuthenticationService _bffAuthenticationService;

    public RefreshTokenHandler(IBffAuthenticationService bffAuthenticationService)
    {
        _bffAuthenticationService = bffAuthenticationService;
    }

    public Task<RefreshTokenResult> HandleAsync(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        return _bffAuthenticationService.RefreshTokenAsync(
            request.SessionId,
            request.CsrfCookieToken,
            request.CsrfHeaderToken,
            cancellationToken
        );
    }
}
