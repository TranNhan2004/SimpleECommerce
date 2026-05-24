using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

public class ActivateMyProfileHandler : IUseCaseHandler<ActivateMyProfileCommand>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateMyProfileHandler(
        IUserContextHolder userContextHolder,
        IUserService userService,
        IUnitOfWork unitOfWork
    )
    {
        _userContextHolder = userContextHolder;
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        ActivateMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var user = await _userService.GetByIdForUpdateAsync(currentUser.Id);

        user.Activate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _userService.InvalidateCacheAsync(
            exactKeys: [UserCacheKeys.GetProfileKey(user.Id), PermissionCacheKeys.GetPermissionSetKey(user.Id)]
        );
    }
}
