using MediatR;
using SimpleECommerceBackend.Application.UseCases.User.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.User.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileDto?>;

[AutoConstructor]
public partial class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public async Task<UserProfileDto?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // Repo FindByIdAsync? Or FindByCredentialId? UserProfile Id is usually NOT CredentialId.
        // But UserId in token usually maps to UserProfileId or CredentialId.
        // Assuming request.UserId is UserProfileId.
        var profile = await _userProfileRepository.FindByIdAsync(request.UserId);
        
        if (profile is null) return null;

        return new UserProfileDto(
            profile.Id,
            profile.Email,
            profile.FirstName,
            profile.LastName,
            profile.NickName,
            profile.Sex,
            profile.BirthDate,
            profile.AvatarUrl
        );
    }
}
