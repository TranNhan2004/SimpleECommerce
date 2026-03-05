using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Auth.RefreshToken;

namespace SimpleECommerceBackend.Application.UseCases.Auth.RefreshToken;

[AutoConstructor]
public partial class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IKeycloakTokenService _keycloakTokenService;

    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        var tokenResponse = await _keycloakTokenService.RefreshTokenAsync(
            request.RefreshToken,
            cancellationToken
        );

        return new RefreshTokenResult
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn
        };
    }
}