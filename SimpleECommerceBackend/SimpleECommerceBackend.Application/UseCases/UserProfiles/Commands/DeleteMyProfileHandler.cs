using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

[AutoConstructor]
public partial class DeleteMyProfileHandler : IUseCaseHandler<DeleteMyProfileCommand>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;

    public async Task HandleAsync(
        DeleteMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var userProfile = await _userProfileService.GetByIdForUpdateAsync(currentUser.Id);

        userProfile.Archived();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _userProfileService.InvalidateCacheAsync(
            exactKeys: [UserProfileCacheKeys.GetProfileKey(userProfile.Id)]
        );
    }
}
