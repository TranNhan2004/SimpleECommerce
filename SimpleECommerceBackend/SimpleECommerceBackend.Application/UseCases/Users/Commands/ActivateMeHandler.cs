using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Events;
using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.UseCases.Users.Commands;

public class ActivateMeHandler : IUseCaseHandler<ActivateMeCommand>
{
    private readonly Serilog.ILogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateMeHandler(
        Serilog.ILogger logger,
        IEventDispatcher eventDispatcher,
        ICurrentUserContext currentUserContext,
        IUserService userService,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;
        _currentUserContext = currentUserContext;
        _userService = userService;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        ActivateMeCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userService.GetByIdForUpdateAsync(_currentUserContext.Id);

        user.Status = UserStatus.Active;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _eventDispatcher.SendAsync(new RemoveCacheEventModel
        {
            Keys =
            [
                UserCacheKeys.GetUserByIdKey(user.Id),
                PermissionCacheKeys.GetPermissionSetKey(user.Id)
            ]
        }, cancellationToken);
    }
}
