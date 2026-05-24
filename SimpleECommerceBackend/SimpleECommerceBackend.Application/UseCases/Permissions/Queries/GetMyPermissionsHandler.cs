using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Permissions;

namespace SimpleECommerceBackend.Application.UseCases.Permissions.Queries;

public class GetMyPermissionsHandler : IUseCaseHandler<GetMyPermissionsQuery, GetMyPermissionsResult>
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IPermissionService _permissionService;

    public GetMyPermissionsHandler(
        IUserContextHolder userContextHolder,
        IPermissionService permissionService
    )
    {
        _userContextHolder = userContextHolder;
        _permissionService = permissionService;
    }

    public async Task<GetMyPermissionsResult> HandleAsync(
        GetMyPermissionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUser = _userContextHolder.GetUserContext();
        var permissions = await _permissionService.GetPermissionCodesByUserIdAsync(currentUser.Id, cancellationToken);

        return new GetMyPermissionsResult
        {
            Permissions = permissions
        };
    }
}
