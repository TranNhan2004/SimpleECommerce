using SimpleECommerceBackend.Application.Interfaces.Services.Auth;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Commands;

public class CompleteLoginHandler : IUseCaseHandler<CompleteLoginCommand, CompleteLoginResult>
{
    private readonly IBffAuthenticationService _bffAuthenticationService;

    public CompleteLoginHandler(IBffAuthenticationService bffAuthenticationService)
    {
        _bffAuthenticationService = bffAuthenticationService;
    }

    public Task<CompleteLoginResult> HandleAsync(
        CompleteLoginCommand request,
        CancellationToken cancellationToken
    )
    {
        return _bffAuthenticationService.CompleteLoginAsync(request, cancellationToken);
    }
}
