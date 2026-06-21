using SimpleECommerceBackend.Application.Interfaces.Services.Auth;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Commands;

public class LogoutHandler : IUseCaseHandler<LogoutCommand, LogoutResult>
{
    private readonly IBffAuthenticationService _bffAuthenticationService;

    public LogoutHandler(IBffAuthenticationService bffAuthenticationService)
    {
        _bffAuthenticationService = bffAuthenticationService;
    }

    public Task<LogoutResult> HandleAsync(
        LogoutCommand request,
        CancellationToken cancellationToken
    )
    {
        return _bffAuthenticationService.LogoutAsync(
            request.SessionId,
            request.CsrfCookieToken,
            request.CsrfHeaderToken,
            cancellationToken
        );
    }
}
