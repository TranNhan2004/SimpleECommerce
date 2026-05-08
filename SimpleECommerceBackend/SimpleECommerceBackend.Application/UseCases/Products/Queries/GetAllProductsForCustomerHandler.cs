using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Products;

namespace SimpleECommerceBackend.Application.UseCases.Products;

public class GetAllProductsForCustomerHandler : IUseCaseHandler<GetAllProductsQueryForCustomer, GetAllProductsResultForCustomer>
{
    private readonly IProductService _productService;

    public GetAllProductsForCustomerHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<GetAllProductsResultForCustomer> HandleAsync(
        GetAllProductsQueryForCustomer request,
        CancellationToken cancellationToken
    )
    {
        return await _productService.GetAllProductsForCustomerAsync(request);
    }
}