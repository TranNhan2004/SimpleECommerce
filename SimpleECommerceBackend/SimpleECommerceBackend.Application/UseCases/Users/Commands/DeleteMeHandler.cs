using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Events;
using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.UseCases.Users.Commands;

public class DeleteMeHandler : IUseCaseHandler<DeleteMeCommand>
{
    private readonly Serilog.ILogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMeHandler(
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
        DeleteMeCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userService.GetByIdForUpdateAsync(_currentUserContext.Id);

        user.SoftDelete(_currentUserContext.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _eventDispatcher.SendAsync(new RemoveCacheEventModel
        {
            Keys = [UserCacheKeys.GetUserByIdKey(user.Id), PermissionCacheKeys.GetPermissionSetKey(user.Id)]
        }, cancellationToken);
    }
}
