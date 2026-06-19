using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Events;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

public class UpdateMyProfileHandler : IUseCaseHandler<UpdateMyProfileCommand, UpdateMyProfileResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICurrentUserContextProvider _userContextHolder;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMyProfileHandler(
        Serilog.ILogger logger,
        IEventDispatcher eventDispatcher,
        ICurrentUserContextProvider userContextHolder,
        IUserService userService,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;
        _userContextHolder = userContextHolder;
        _userService = userService;
        _unitOfWork = unitOfWork;
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

        await _eventDispatcher.SendAsync(new RemoveCacheEventModel
        {
            Keys = [UserCacheKeys.GetUserByIdKey(currentUser.Id)]
        }, cancellationToken);

        return UpdateMyProfileResult.FromEntity(user);
    }
}
