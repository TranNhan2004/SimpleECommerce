using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.Authorization;
using SimpleECommerceBackend.Api.DTOs.Errors;
using SimpleECommerceBackend.Api.DTOs.V1_0.Categories;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Controllers.V1_0;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/categories")]
[ApiVersion("1.0")]
[ApiController]
[AutoConstructor]
public partial class CategoryController : ControllerBase
{
    private readonly IUseCaseDispatcher _dispatcher;
    private readonly IMapper _mapper;

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<GetAllCategoriesResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllCategoriesAsync([FromQuery] GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = _mapper.Map<GetAllCategoriesQuery>(request);
        var result = await _dispatcher.SendAsync<GetAllCategoriesQuery, IReadOnlyList<GetAllCategoriesResult>>(query, cancellationToken);
        var response = _mapper.Map<IReadOnlyList<GetAllCategoriesResponse>>(result);
        return Ok(response);
    }

    [HttpGet("for-admin")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<GetAllCategoriesForAdminResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllCategoriesForAdminAsync([FromQuery] GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = _mapper.Map<GetAllCategoriesQuery>(request);
        var result = await _dispatcher.SendAsync<GetAllCategoriesQuery, IReadOnlyList<GetAllCategoriesResult>>(query, cancellationToken);
        var response = _mapper.Map<IReadOnlyList<GetAllCategoriesForAdminResponse>>(result);
        return Ok(response);
    }

    [HttpPost("for-admin")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateCategoryForAdminResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateCategoryForAdminAsync([FromBody] CreateCategoryForAdminRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateCategoryCommand>(request);
        var result = await _dispatcher.SendAsync<CreateCategoryCommand, CreateCategoryResult>(command, cancellationToken);
        var response = _mapper.Map<CreateCategoryForAdminResponse>(result);
        return Ok(response);
    }
}