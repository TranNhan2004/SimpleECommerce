using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Queries;

[AutoConstructor]
public partial class GetMyProfileHandler : IUseCaseHandler<GetMyProfileQuery, GetMyProfileResult>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;

    public async Task<GetMyProfileResult> HandleAsync(
        GetMyProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var userProfile = await _userProfileService.GetByIdAsync(currentUser.Id);

        return new GetMyProfileResult
        {
            Id = userProfile.Id,
            Email = userProfile.Email,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            NickName = userProfile.NickName,
            Sex = SexUtils.ToName(userProfile.Sex),
            Status = UserStatusUtils.ToName(userProfile.Status),
            BirthDate = userProfile.BirthDate
        };
    }
}