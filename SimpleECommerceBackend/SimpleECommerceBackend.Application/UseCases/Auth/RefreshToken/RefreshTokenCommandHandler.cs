using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.UseCases.Auth.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<RefreshTokenResult>;

public class RefreshTokenResult
{
    public string AccessToken { get; init; } = null!;
}

[AutoConstructor]
public partial class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IJwtGenerator _jwtGenerator;

    public Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        var claimsInfo = _jwtGenerator.ValidateToken(request.RefreshToken);
        var role = claimsInfo.Role ?? throw new UnauthorizedException("Invalid token");
        if (claimsInfo.TokenType != TokenType.RefreshToken)
            throw new UnauthorizedException("Invalid token");
        
        var accessToken = _jwtGenerator.GenerateAccessToken(claimsInfo.Email, role);
        return Task.FromResult(new RefreshTokenResult
        {
            AccessToken = accessToken
        });
    }
}