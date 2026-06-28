using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Permissions;

namespace SimpleECommerceBackend.Application.UseCases.Permissions.Query;

public class GetMyPermissionsHandler : IUseCaseHandler<GetMyPermissionsQuery, GetMyPermissionsResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IPermissionService _permissionService;

    public GetMyPermissionsHandler(
        Serilog.ILogger logger,
        ICurrentUserContext currentUserContext,
        IPermissionService permissionService
    )
    {
        _logger = logger;
        _currentUserContext = currentUserContext;
        _permissionService = permissionService;
    }

    public async Task<GetMyPermissionsResult> HandleAsync(
        GetMyPermissionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var codes = await _permissionService.GetPermissionCodesByUserIdAsync(_currentUserContext.Id);
        return new GetMyPermissionsResult
        {
            Permissions = codes
        };
    }
}