using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.Attributes;
using SimpleECommerceBackend.Api.Dtos.Common.Errors;
using SimpleECommerceBackend.Api.Dtos.V1.Permissions;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Permissions;
using SimpleECommerceBackend.Domain.Constants.Permissions;

namespace SimpleECommerceBackend.Api.Controllers.V1;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/permissions")]
[ApiVersion("1.0")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly IUseCaseDispatcher _dispatcher;

    public PermissionController(IUseCaseDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("me")]
    [Authorize]
    [AllowPermissions(PermissionCodes.PermissionsSelfRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMyPermissionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetMyPermissionsAsync(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync<GetMyPermissionsQuery, GetMyPermissionsResult>(
            new GetMyPermissionsQuery(),
            cancellationToken
        );
        return Ok(GetMyPermissionsResponse.FromResult(result));
    }
}
