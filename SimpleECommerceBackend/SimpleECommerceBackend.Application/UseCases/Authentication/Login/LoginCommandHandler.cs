using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResult>;

public sealed class LoginResult
{
    public Guid CredentialId { get; init; }
    public string Email { get; init; } = string.Empty;
    public Role Role { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}

[AutoConstructor]
public sealed partial class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public async Task<LoginResult> Handle(
        LoginCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var credential = await _credentialRepository.FindByEmailAsync(cmd.Email);

        if (credential is null)
            throw new UnauthorizedException("Invalid email or password");

        if (credential.Status != CredentialStatus.Active)
            throw new ForbiddenException(
                resource: "Credential",
                action: "Login",
                message: "You are not authorized to access this resource."
            );

        if (!_passwordHasher.Verify(cmd.Password, credential.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(
            credential.Id,
            credential.Email,
            credential.Role
        );
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        return new LoginResult
        {
            CredentialId = credential.Id,
            Email = credential.Email,
            Role = credential.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}