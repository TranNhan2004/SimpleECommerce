using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Queries;

public class GetMyProfileHandler : IUseCaseHandler<GetMyProfileQuery, GetMyProfileResult>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserService _userService;

    public GetMyProfileHandler(
        IUserContextHolder userContextHolder,
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
