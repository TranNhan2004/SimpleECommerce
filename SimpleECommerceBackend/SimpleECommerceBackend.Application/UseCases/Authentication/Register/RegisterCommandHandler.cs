using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Register;

[AutoConstructor]
public sealed partial class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<RegisterResult> Handle(
        RegisterCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        var credential = Credential.Create(
            cmd.Email,
            _passwordHasher.Hash(cmd.Password),
            cmd.RoleId
        );

        _credentialRepository.Add(credential);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterResult
        {
            CredentialId = credential.Id
        };
    }
}