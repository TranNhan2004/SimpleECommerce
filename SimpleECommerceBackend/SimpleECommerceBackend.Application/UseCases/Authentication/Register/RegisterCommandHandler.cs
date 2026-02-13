using MediatR;
using SimpleECommerceBackend.Application.Events.Email;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;
using SimpleECommerceBackend.Domain.Entities.Auth;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Auth;

namespace SimpleECommerceBackend.Application.UseCases.Authentication.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    Role Role
) : IRequest<RegisterResult>;

public sealed class RegisterResult
{
    public Guid CredentialId { get; init; }
}

[AutoConstructor]
public sealed partial class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IEmailService _emailService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMediator _mediator;
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
            cmd.Role
        );

        _credentialRepository.Add(credential);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _emailService.SendAsync(
            cmd.Email,
            "Welcome to SimpleECommerce",
            "Thanks for registering!"
        );

        var verificationUrl = _jwtTokenGenerator.GenerateAccountVerificationToken(credential.Id);

        await _mediator.Publish(
            new UserRegisteredEvent(credential.Email, verificationUrl),
            cancellationToken
        );

        return new RegisterResult
        {
            CredentialId = credential.Id
        };
    }
}