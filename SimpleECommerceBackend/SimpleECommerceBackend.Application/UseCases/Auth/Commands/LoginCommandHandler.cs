using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Auth;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Commands;

[AutoConstructor]
public partial class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IKeycloakTokenService _keycloakTokenService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validatedEmail = CredentialUtils.ValidateEmail(request.Email);
        var validatedPassword = CredentialUtils.ValidatePassword(request.Password);

        var tokenResponse = await _keycloakTokenService.GetTokenAsync(
            validatedEmail,
            validatedPassword,
            cancellationToken
        );

        var userInfo = await _keycloakTokenService.GetUserInfoAsync(
            tokenResponse.AccessToken,
            cancellationToken
        );

        var keycloakUserId = Guid.Parse(userInfo.Sub);
        var userProfile = await _userProfileRepository.FindByIdAsync(keycloakUserId);

        if (userProfile is null)
        {
            userProfile = UserProfile.Create(
                keycloakUserId,
                userInfo.Email,
                string.IsNullOrWhiteSpace(userInfo.GivenName) ? "User" : userInfo.GivenName,
                string.IsNullOrWhiteSpace(userInfo.FamilyName) ? "Name" : userInfo.FamilyName,
                null,
                Sex.Other,
                AgeUtils.CreateRandomBirthDate(UserProfileConstants.MinAge, UserProfileConstants.MaxAge),
                null
            );
            _userProfileRepository.Add(userProfile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var role = RoleUtils.ResolvePrimaryRole(userInfo.Roles);

        return new LoginResult
        {
            UserId = userProfile.Id,
            Email = userProfile.Email,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            NickName = userProfile.NickName,
            Sex = userProfile.Sex,
            BirthDate = userProfile.BirthDate,
            AvatarUrl = userProfile.AvatarUrl,
            Role = role,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn
        };
    }
}