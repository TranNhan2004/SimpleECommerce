using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Queries;

public class GetMyProfileHandler : IUseCaseHandler<GetMyProfileQuery, GetMyProfileResult>
{
    private readonly ICurrentUserContextProvider _userContextHolder;
    private readonly IUserService _userService;

    public GetMyProfileHandler(
        ICurrentUserContextProvider userContextHolder,
        IUserService userService
    )
    {
        _userContextHolder = userContextHolder;
        _userService = userService;
    }

    public async Task<GetMyProfileResult> HandleAsync(
        GetMyProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var user = await _userService.GetByIdAsync(currentUser.Id);

        return GetMyProfileResult.FromEntity(user);
    }
}
