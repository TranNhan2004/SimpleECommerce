using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

public class ActivateMyProfileHandler : IUseCaseHandler<ActivateMyProfileCommand>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateMyProfileHandler(
        IUserContextHolder userContextHolder,
        IUserProfileService userProfileService,
        IUnitOfWork unitOfWork
    )
    {
        _userContextHolder = userContextHolder;
        _userProfileService = userProfileService;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        ActivateMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var userProfile = await _userProfileService.GetByIdForUpdateAsync(currentUser.Id);

        userProfile.Activate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}