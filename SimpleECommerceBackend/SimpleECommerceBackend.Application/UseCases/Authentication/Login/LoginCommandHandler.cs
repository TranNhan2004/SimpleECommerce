using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Login;

[AutoConstructor]
public sealed partial class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IPasswordHasher _passwordHasher;

    public async Task<LoginResult> Handle(
        LoginCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var credential =
            await _credentialRepository.FindByEmailAsync(cmd.Email);

        if (credential is null)
            throw new UnauthorizedException("Invalid email or password");

        if (credential.Status != CredentialStatus.Active)
            throw new ForbiddenException(
                resource: "Credential",
                action: "Login",
                message: "You are not authorized to access this resource.");

        if (!_passwordHasher.Verify(cmd.Password, credential.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        return new LoginResult
        {
            CredentialId = credential.Id,
            Email = credential.Email,
            Role = credential.Role
        };
    }
}