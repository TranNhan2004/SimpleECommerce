using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Events.Email;
using SimpleECommerceBackend.Application.Models.Auth;
using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Commands;

[AutoConstructor]
public partial class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IEmailVerificationRepository _emailVerificationRepository;
    private readonly IKeycloakAdminService _keycloakAdminService;
    private readonly IPublisher _publisher;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _keycloakAdminService.UserExistsAsync(request.Email, cancellationToken);
        if (userExists)
            throw new ConflictException(
                "UserProfile",
                "email",
                request.Email,
                "User with this email already exists"
            );

        var role = RoleUtils.Parse(request.Role);

        var keycloakUser = await _keycloakAdminService.CreateUserAsync(new CreateKeycloakUserRequest
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = role
        }, cancellationToken);

        var verificationToken = TokenUtils.CreateVerificationToken();
        var emailVerification = EmailVerification.Create(
            Guid.Parse(keycloakUser.KeycloakUserId),
            request.Email,
            TokenUtils.HashToken(verificationToken),
            DateTimeOffset.UtcNow.AddHours(EmailVerificationConstants.TokenLifetimeHours)
        );

        var userProfile = UserProfile.Create(
            Guid.Parse(keycloakUser.KeycloakUserId),
            request.Email,
            request.FirstName,
            request.LastName,
            null,
            Sex.Other,
            AgeUtils.CreateRandomBirthDate(UserProfileConstants.MinAge, UserProfileConstants.MaxAge),
            null
        );

        _emailVerificationRepository.Add(emailVerification);
        _userProfileRepository.Add(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(
            new SendEmailVerificationEvent(request.Email, verificationToken),
            cancellationToken
        );

        return new RegisterResult
        {
            Email = request.Email
        };
    }
}