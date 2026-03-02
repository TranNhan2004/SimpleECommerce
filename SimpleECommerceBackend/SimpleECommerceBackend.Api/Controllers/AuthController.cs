using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleECommerceBackend.Api.DTOs.Auth;
using SimpleECommerceBackend.Api.DTOs.Errors;
using SimpleECommerceBackend.Application.UseCases.Auth.Login;
using SimpleECommerceBackend.Application.UseCases.Auth.Register;

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
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request)
    {
        var result = await _sender.Send(request);
        var response = _mapper.Map<RegisterResult, RegisterResponse>(result);
        return Ok(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand request)
    {
        var result = await _sender.Send(request);
        var response = _mapper.Map<LoginResult, LoginResponse>(result);
        return Ok(response);
    }
}