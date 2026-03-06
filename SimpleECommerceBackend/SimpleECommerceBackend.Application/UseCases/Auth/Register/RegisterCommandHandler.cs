using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Auth.Register;
using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Register;

[AutoConstructor]
public partial class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IKeycloakAdminService _keycloakAdminService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists in Keycloak
        var userExists = await _keycloakAdminService.UserExistsAsync(request.Email, cancellationToken);
        if (userExists)
            throw new BusinessException("User with this email already exists");

        // Validate role
        var validRoles = new[] { "customer", "seller", "admin" };
        if (!validRoles.Contains(request.Role.ToLower()))
            throw new BusinessException($"Invalid role. Must be one of: {string.Join(", ", validRoles)}");

        // Create user in Keycloak
        var keycloakUser = await _keycloakAdminService.CreateUserAsync(new CreateKeycloakUserRequest
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role.ToLower()
        }, cancellationToken);

        // Create local UserProfile with Keycloak user ID
        var userProfile = UserProfile.Create(
            Guid.Parse(keycloakUser.KeycloakUserId),
            request.Email,
            request.FirstName,
            request.LastName,
            null, // nickname
            Sex.Other,
            AgeUtils.CreateRandomBirthDate(UserProfileConstants.MinAge, UserProfileConstants.MaxAge),
            null // avatar URL
        );

        _userProfileRepository.Add(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterResult
        {
            Email = request.Email
        };
    }
}