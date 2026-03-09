using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleECommerceBackend.Api.DTOs.Auth;
using SimpleECommerceBackend.Api.DTOs.Errors;
using SimpleECommerceBackend.Application.Models.Users.Update;
using SimpleECommerceBackend.Application.Models.Auth.ConfirmEmail;
using SimpleECommerceBackend.Application.Models.Auth.RefreshToken;
using SimpleECommerceBackend.Application.Models.Auth.Register;
using SimpleECommerceBackend.Application.Models.Auth.Login;

namespace SimpleECommerceBackend.Api.Controllers;


[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
[ApiController]
[AutoConstructor]
public partial class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<RegisterResponse>(result);
        return Ok(response);
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ConfirmEmailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
    {
        var command = _mapper.Map<ConfirmEmailCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<ConfirmEmailResponse>(result);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = _mapper.Map<LoginCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<LoginResponse>(result);
        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = _mapper.Map<RefreshTokenCommand>(request);
        var result = await _sender.Send(command);
        var response = _mapper.Map<RefreshTokenResponse>(result);
        return Ok(response);
    }
}