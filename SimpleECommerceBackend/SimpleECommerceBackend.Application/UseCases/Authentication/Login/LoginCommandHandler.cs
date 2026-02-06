using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Results;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Login;

[AutoConstructor]
public sealed partial class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IPasswordHasher _passwordHasher;

    public async Task<Result<LoginResult>> Handle(
        LoginCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var credential = await _credentialRepository.FindByEmailAsync(cmd.Email);

        if (credential is null)
            return Result<LoginResult>.Fail(AuthenticationErrors.InvalidCredentials);

        if (credential.Status != CredentialStatus.Active)
            return Result<LoginResult>.Fail(AuthenticationErrors.AccountInactive);

        if (!_passwordHasher.Verify(cmd.Password, credential.PasswordHash))
            return Result<LoginResult>.Fail(AuthenticationErrors.InvalidCredentials);

        return Result<LoginResult>.Ok(new LoginResult
        {
            CredentialId = credential.Id,
            Email = credential.Email,
            RoleId = credential.RoleId
        });
    }
}