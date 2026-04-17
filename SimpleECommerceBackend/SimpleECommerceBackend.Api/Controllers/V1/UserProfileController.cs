using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.DTOs.Errors;
using SimpleECommerceBackend.Api.DTOs.V1.UserProfiles;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Api.Controllers.V1;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/user-profiles")]
[ApiVersion("1.0")]
[ApiController]
[AutoConstructor]
public partial class UserProfileController : ControllerBase
{
    private readonly IUseCaseDispatcher _dispatcher;
    private readonly IMapper _mapper;

    [HttpPost("me/info")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateMyProfileResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateMyProfileAsync([FromBody] CreateMyProfileRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateMyProfileCommand>(request);
        var result = await _dispatcher.SendAsync<CreateMyProfileCommand, CreateMyProfileResult>(command, cancellationToken);
        var response = _mapper.Map<CreateMyProfileResponse>(result);
        return Ok(response);
    }

    [HttpGet("me/info")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMyProfileResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetMyInfoAsync([FromQuery] GetMyProfileRequest request, CancellationToken cancellationToken)
    {
        var query = _mapper.Map<GetMyProfileQuery>(request);
        var result = await _dispatcher.SendAsync<GetMyProfileQuery, GetMyProfileResult>(query, cancellationToken);
        var response = _mapper.Map<GetMyProfileResponse>(result);
        return Ok(response);
    }

    [HttpPut("me/info")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateMyProfileResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateMyInfoAsync([FromBody] UpdateMyProfileRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateMyProfileCommand>(request);
        var result = await _dispatcher.SendAsync<UpdateMyProfileCommand, UpdateMyProfileResult>(command, cancellationToken);
        var response = _mapper.Map<UpdateMyProfileResponse>(result);
        return Ok(response);
    }

    [HttpDelete("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> DeleteMyProfileAsync([FromBody] DeleteMyProfileRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<DeleteMyProfileCommand>(request);
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
    public async Task<IActionResult> ActivateMyProfileAsync([FromBody] ActivateMyProfileRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<ActivateMyProfileCommand>(request);
        await _dispatcher.SendAsync(command, cancellationToken);
        return NoContent();
    }
}