using SimpleECommerceBackend.Application.Interfaces.Services.Auth;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Commands;

public class BeginLoginHandler : IUseCaseHandler<BeginLoginCommand, BeginLoginResult>
{
    private readonly IBffAuthenticationService _bffAuthenticationService;

    public BeginLoginHandler(IBffAuthenticationService bffAuthenticationService)
    {
        _bffAuthenticationService = bffAuthenticationService;
    }

    public Task<BeginLoginResult> HandleAsync(
        BeginLoginCommand request,
        CancellationToken cancellationToken
    )
    {
        return _bffAuthenticationService.BeginLoginAsync(request.ReturnUrl, cancellationToken);
    }
}
