using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Events;
using SimpleECommerceBackend.Application.Models.UserProfiles;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

public class DeleteMyProfileHandler : IUseCaseHandler<DeleteMyProfileCommand>
{
    private readonly Serilog.ILogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICurrentUserContextProvider _userContextHolder;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMyProfileHandler(
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

    public async Task HandleAsync(
        DeleteMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var user = await _userService.GetByIdForUpdateAsync(currentUser.Id);

        user.SoftDelete(currentUser.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _eventDispatcher.SendAsync(new RemoveCacheEventModel
        {
            Keys = [UserCacheKeys.GetUserByIdKey(user.Id), PermissionCacheKeys.GetPermissionSetKey(user.Id)]
        }, cancellationToken);
    }
}
