using MediatR;
using SimpleECommerceBackend.Application.Events.Email;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Register;

public record RegisterCommand(
    string Email,
    string Password,
    Role Role
) : IRequest<RegisterResult>;

public class RegisterResult
{
    public string Email { get; init; } = null!;
}

[AutoConstructor]
public partial class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IMediator _mediator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken = default)
    {
        var credential = Credential.Create(
            request.Email,
            _passwordHasher.Hash(request.Password),
            request.Role
        );

        _credentialRepository.Add(credential);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var verificationUrl = _jwtGenerator.GenerateAccountVerificationToken(credential.Email);

        await _mediator.Publish(
            new UserRegisteredEvent(credential.Email, verificationUrl),
            cancellationToken
        );

        return new RegisterResult
        {
            Email = credential.Email
        };
    }
}