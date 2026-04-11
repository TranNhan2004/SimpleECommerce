using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.DTOs.Errors;
using SimpleECommerceBackend.Api.DTOs.UserProfiles;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Api.Controllers;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/user-profiles")]
[ApiVersion("1.0")]
[ApiController]
[Authorize]
[AutoConstructor]
public partial class UserProfileController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    [HttpPut("me/info")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateMyProfileResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateMyInfoAsync([FromBody] UpdateMyProfileRequest request)
    {
        var command = _mapper.Map<UpdateMyProfileCommand>(request);
        var result = await _mediator.Send(command);
        var response = _mapper.Map<UpdateMyProfileResponse>(result);
        return Ok(response);
    }
}