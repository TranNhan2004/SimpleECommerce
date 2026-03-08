using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Auth.Login;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Login;

[AutoConstructor]
public partial class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IKeycloakTokenService _keycloakTokenService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        var tokenResponse = await _keycloakTokenService.GetTokenAsync(
            request.Email,
            request.Password,
            cancellationToken
        );

        var userInfo = await _keycloakTokenService.GetUserInfoAsync(
            tokenResponse.AccessToken,
            cancellationToken
        );

        var keycloakUserId = Guid.Parse(userInfo.Sub);
        var userProfile = await _userProfileRepository.FindByIdAsync(keycloakUserId);

        if (userProfile == null)
        {
            userProfile = UserProfile.Create(
                keycloakUserId,
                userInfo.Email,
                userInfo.GivenName ?? "User",
                userInfo.FamilyName ?? "Name",
                null,
                Sex.Other,
                AgeUtils.CreateRandomBirthDate(UserProfileConstants.MinAge, UserProfileConstants.MaxAge),
                null
            );
            _userProfileRepository.Add(userProfile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var role = userInfo.Roles.FirstOrDefault() ?? "customer";

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