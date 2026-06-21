using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Events;
using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;

namespace SimpleECommerceBackend.Application.UseCases.Users.Commands;

public class UpdateMeHandler : IUseCaseHandler<UpdateMeCommand, UpdateMeResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMeHandler(
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

    public async Task<UpdateMeResult> HandleAsync(
        UpdateMeCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userService.GetByIdForUpdateAsync(_currentUserContext.Id);

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
            Keys = [UserCacheKeys.GetUserByIdKey(_currentUserContext.Id)]
        }, cancellationToken);

        return UpdateMeResult.FromEntity(user);
    }
}
