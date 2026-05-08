using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;
namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Queries;

public partial class GetMyProfileHandler : IUseCaseHandler<GetMyProfileQuery, GetMyProfileResult>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;

    public GetMyProfileHandler(
        IUserContextHolder userContextHolder,
        IUserProfileService userProfileService
    )
    {
        _userContextHolder = userContextHolder;
        _userProfileService = userProfileService;
    }

    public async Task<GetMyProfileResult> HandleAsync(
        GetMyProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var userProfile = await _userProfileService.GetByIdAsync(currentUser.Id);

        return GetMyProfileResult.FromEntity(userProfile);
    }
}
