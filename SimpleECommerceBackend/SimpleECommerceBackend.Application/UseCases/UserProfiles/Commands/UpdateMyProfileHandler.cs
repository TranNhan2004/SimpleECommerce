using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

[AutoConstructor]
public partial class UpdateMyProfileHandler : IUseCaseHandler<UpdateMyProfileCommand, UpdateMyProfileResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;

    public async Task<UpdateMyProfileResult> HandleAsync(
        UpdateMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var userProfile = await _userProfileService.GetByIdForUpdateAsync(currentUser.Id);

        userProfile.SetNickName(request.NickName);

        if (request.FirstName is not null)
            userProfile.SetFirstName(request.FirstName);
        if (request.LastName is not null)
            userProfile.SetLastName(request.LastName);
        if (request.Sex.HasValue)
            userProfile.SetSex(request.Sex.Value);
        if (request.BirthDate.HasValue)
            userProfile.SetBirthDate(request.BirthDate.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _userProfileService.InvalidateCacheAsync(
            exactKeys: [UserProfileCacheKeys.GetProfileKey(userProfile.Id)]
        );
        return UpdateMyProfileResult.FromEntity(userProfile);
    }
}
