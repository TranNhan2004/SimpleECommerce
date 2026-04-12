using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.DTOs.Errors;
using SimpleECommerceBackend.Api.DTOs.V1_0.Category;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;

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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<GetAllCategoriesResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllCategoriesAsync([FromQuery] GetAllCategoriesRequest request)
    {
        var query = _mapper.Map<GetAllCategoriesQuery>(request);
        var result = await _dispatcher.SendAsync<GetAllCategoriesQuery, GetAllCategoriesResult>(query);
        var response = _mapper.Map<GetAllCategoriesResponse>(result);
        return Ok(response);
    }

    [HttpGet("for-amdin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<GetAllCategoriesForAdminResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllCategoriesForAdminAsync([FromQuery] GetAllCategoriesRequest request)
    {
        var query = _mapper.Map<GetAllCategoriesQuery>(request);
        var result = await _dispatcher.SendAsync<GetAllCategoriesQuery, GetAllCategoriesResult>(query);
        var response = _mapper.Map<GetAllCategoriesForAdminResponse>(result);
        return Ok(response);
    }
}