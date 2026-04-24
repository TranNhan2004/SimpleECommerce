using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.Authorization;
using SimpleECommerceBackend.Api.Dtos.Common.Errors;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Api.Dtos.V1.Categories;

namespace SimpleECommerceBackend.Api.Controllers.V1;

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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllCategoriesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllCategoriesAsync([FromQuery] GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = GetAllCategoriesRequest.ToQuery(request);
        var result = await _dispatcher.SendAsync<GetAllCategoriesQuery, GetAllCategoriesResult>(query, cancellationToken);
        var response = GetAllCategoriesResponse.FromResult(result);
        return Ok(response);
    }

    [HttpGet("for-admin")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SimpleECommerceBackend.Api.Dtos.V1.Categories.GetAllCategoriesForAdminResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllCategoriesForAdminAsync([FromQuery] GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = GetAllCategoriesRequest.ToQuery(request);
        var result = await _dispatcher.SendAsync<GetAllCategoriesQuery, GetAllCategoriesResult>(query, cancellationToken);
        var response = SimpleECommerceBackend.Api.Dtos.V1.Categories.GetAllCategoriesForAdminResponse.FromResult(result);
        return Ok(response);
    }

    [HttpPost("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryQuery { Id = id };
        var result = await _dispatcher.SendAsync<GetCategoryQuery, GetCategoryResult>(query, cancellationToken);
        var response = _mapper.Map<GetCategoryResponse>(result);
        return Ok(response);
    }

    [HttpGet("{id}/for-admin")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCategoryForAdminResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetCategoryForAdminAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryQuery { Id = id };
        var result = await _dispatcher.SendAsync<GetCategoryQuery, GetCategoryResult>(query, cancellationToken);
        var response = _mapper.Map<GetCategoryForAdminResponse>(result);
        return Ok(response);
    }

    [HttpPost("for-admin")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateCategoryCommand>(request);
        var result = await _dispatcher.SendAsync<CreateCategoryCommand, CreateCategoryResult>(command, cancellationToken);
        var response = _mapper.Map<CreateCategoryResponse>(result);
        return Ok(response);
    }

    [HttpPut("for-admin")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateCategoryCommand>(request);
        var result = await _dispatcher.SendAsync<UpdateCategoryCommand, UpdateCategoryResult>(command, cancellationToken);
        var response = _mapper.Map<UpdateCategoryResponse>(result);
        return Ok(response);
    }

    [HttpDelete("{id}/for-admin")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand { Id = id };
        await _dispatcher.SendAsync(command, cancellationToken);
        return NoContent();
    }
}
