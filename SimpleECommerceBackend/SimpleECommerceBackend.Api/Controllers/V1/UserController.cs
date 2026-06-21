using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.Attributes;
using SimpleECommerceBackend.Api.Dtos.Common.Errors;
using SimpleECommerceBackend.Api.Dtos.V1.Users;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Constants.Permissions;

namespace SimpleECommerceBackend.Api.Controllers.V1;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1.0")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUseCaseDispatcher _dispatcher;

    public UserController(IUseCaseDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateMeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateMeAsync([FromBody] CreateMeRequest request, CancellationToken cancellationToken)
    {
        var command = CreateMeRequest.ToCommand(request);
        var result = await _dispatcher.SendAsync<CreateMeCommand, CreateMeResult>(command, cancellationToken);
        var response = CreateMeResponse.FromResult(result);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [AllowPermissions(PermissionCodes.UsersSelfRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetMeAsync([FromQuery] GetMeRequest request, CancellationToken cancellationToken)
    {
        var query = GetMeRequest.ToQuery(request);
        var result = await _dispatcher.SendAsync<GetMeQuery, GetMeResult>(query, cancellationToken);
        var response = GetMeResponse.FromResult(result);
        return Ok(response);
    }

    [HttpPut("me")]
    [Authorize]
    [AllowPermissions(PermissionCodes.UsersSelfUpdate)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateMeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateMeAsync([FromBody] UpdateMeRequest request, CancellationToken cancellationToken)
    {
        var command = UpdateMeRequest.ToCommand(request);
        var result = await _dispatcher.SendAsync<UpdateMeCommand, UpdateMeResult>(command, cancellationToken);
        var response = UpdateMeResponse.FromResult(result);
        return Ok(response);
    }

    [HttpDelete("me")]
    [Authorize]
    [AllowPermissions(PermissionCodes.UsersSelfDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> DeleteMeAsync([FromBody] DeleteMeRequest request, CancellationToken cancellationToken)
    {
        var command = DeleteMeRequest.ToCommand(request);
        await _dispatcher.SendAsync(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("me/activation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> ActivateMeAsync([FromBody] ActivateMeRequest request, CancellationToken cancellationToken)
    {
        var command = ActivateMeRequest.ToCommand(request);
        await _dispatcher.SendAsync(command, cancellationToken);
        return NoContent();
    }
}
