using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SimpleECommerceBackend.Api.Dtos.Common.Errors;
using SimpleECommerceBackend.Api.Dtos.V1.Products;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Products;

namespace SimpleECommerceBackend.Api.Controllers.V1;

[EnableRateLimiting("ip-route")]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IUseCaseDispatcher _dispatcher;

    public ProductController(IUseCaseDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("get-all/for-customer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllProductsResponseForCustomer))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrorResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetAllProductsForCustomerAsync([FromBody] GetAllProductsRequestForCustomer request, CancellationToken cancellationToken)
    {
        var query = GetAllProductsRequestForCustomer.ToQuery(request);
        var result = await _dispatcher.SendAsync<GetAllProductsQueryForCustomer, GetAllProductsResultForCustomer>(query, cancellationToken);
        var response = GetAllProductsResponseForCustomer.FromResult(result);
        return Ok(response);
    }

}