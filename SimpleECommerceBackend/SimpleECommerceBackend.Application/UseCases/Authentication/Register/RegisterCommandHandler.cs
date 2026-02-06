using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Results;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Register;

[AutoConstructor]
public sealed partial class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResult>>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<RegisterResult>> Handle(
        RegisterCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var emailExists = await _credentialRepository.ExistsByEmailAsync(cmd.Email);
        if (emailExists)
            return Result<RegisterResult>.Fail(AuthenticationErrors.EmailAlreadyExists);

        var passwordHash = _passwordHasher.Hash(cmd.Password);

        var credential = Credential.Create(
            cmd.Email,
            passwordHash,
            cmd.RoleId
        );

        _credentialRepository.Add(credential);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegisterResult>.Ok(new RegisterResult
        {
            CredentialId = credential.Id
        });
    }
}