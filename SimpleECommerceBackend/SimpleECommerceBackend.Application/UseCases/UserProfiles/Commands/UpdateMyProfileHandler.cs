using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

public class UpdateMyProfileHandler : IUseCaseHandler<UpdateMyProfileCommand, UpdateMyProfileResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContextProvider _userContextHolder;
    private readonly IUserService _userService;

    public UpdateMyProfileHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserContextProvider userContextHolder,
        IUserService userService
    )
    {
        _unitOfWork = unitOfWork;
        _userContextHolder = userContextHolder;
        _userService = userService;
    }

    public async Task<UpdateMyProfileResult> HandleAsync(
        UpdateMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var user = await _userService.GetByIdForUpdateAsync(currentUser.Id);

        user.NickName = request.NickName;

        if (request.FirstName is not null)
            user.FirstName = request.FirstName;
        if (request.LastName is not null)
            user.LastName = request.LastName;
        if (request.Sex.HasValue)
            user.Sex = request.Sex.Value;
        if (request.BirthDate.HasValue)
            user.BirthDate = request.BirthDate.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _userService.InvalidateCacheAsync(
            exactKeys: [UserCacheKeys.GetUserByIdKey(user.Id)]
        );
        return UpdateMyProfileResult.FromEntity(user);
    }
}
