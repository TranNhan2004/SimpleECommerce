using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.Attributes;
using SimpleECommerceBackend.Api.Dtos.Common.Errors;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Api.Dtos.V1.Categories;
using SimpleECommerceBackend.Domain.Constants.Permissions;

namespace SimpleECommerceBackend.Api.Controllers.V1;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/categories")]
[ApiVersion("1.0")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IUseCaseDispatcher _dispatcher;

    public CategoryController(IUseCaseDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("get-all")]
    [Authorize]
    [AllowPermissions(PermissionCodes.CategoriesRead)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllCategoriesResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllCategoriesAsync([FromBody] GetAllCategoriesRequest request, CancellationToken cancellationToken)
    {
        var query = GetAllCategoriesRequest.ToQuery(request);
        var result = await _dispatcher.SendAsync<GetAllCategoriesQuery, GetAllCategoriesResult>(query, cancellationToken);
        var response = GetAllCategoriesResponse.FromResult(result);
        return Ok(response);
    }

    [HttpPost("get-all/for-admin")]
    [Authorize]
    [AllowPermissions(PermissionCodes.CategoriesReadAdmin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllCategoriesForAdminResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllCategoriesForAdminAsync([FromBody] GetAllCategoriesRequestForAdmin request, CancellationToken cancellationToken)
    {
        var query = GetAllCategoriesRequestForAdmin.ToQuery(request);
        var result = await _dispatcher.SendAsync<GetAllCategoriesQueryForAdmin, GetAllCategoriesResultForAdmin>(query, cancellationToken);
        var response = SimpleECommerceBackend.Api.Dtos.V1.Categories.GetAllCategoriesForAdminResponse.FromResult(result);
        return Ok(response);
    }

    [HttpPost("{id}")]
    [Authorize]
    [AllowPermissions(PermissionCodes.CategoriesRead)]
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
        var response = GetCategoryResponse.FromResult(result);
        return Ok(response);
    }

    [HttpGet("{id}/for-admin")]
    [Authorize]
    [AllowPermissions(PermissionCodes.CategoriesReadAdmin)]
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
        var response = GetCategoryForAdminResponse.FromResult(result);
        return Ok(response);
    }

    [HttpPost("for-admin")]
    [Authorize]
    [AllowPermissions(PermissionCodes.CategoriesCreate)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = CreateCategoryRequest.ToCommand(request);
        var result = await _dispatcher.SendAsync<CreateCategoryCommand, CreateCategoryResult>(command, cancellationToken);
        var response = CreateCategoryResponse.FromResult(result);
        return Ok(response);
    }

    [HttpPut("for-admin")]
    [Authorize]
    [AllowPermissions(PermissionCodes.CategoriesUpdate)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = UpdateCategoryRequest.ToCommand(request);
        var result = await _dispatcher.SendAsync<UpdateCategoryCommand, UpdateCategoryResult>(command, cancellationToken);
        var response = UpdateCategoryResponse.FromResult(result);
        return Ok(response);
    }

    [HttpDelete("{id}/for-admin")]
    [Authorize]
    [AllowPermissions(PermissionCodes.CategoriesDelete)]
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
