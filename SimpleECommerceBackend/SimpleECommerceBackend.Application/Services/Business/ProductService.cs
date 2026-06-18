using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Models.Products;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Services.Business;

public class ProductService : IProductService
{
    private readonly Serilog.ILogger _logger;
    private readonly ICacheService _cacheService;
    private readonly IProductRepository _productRepository;

    public ProductService(
        Serilog.ILogger logger,
        ICacheService cacheService,
        IProductRepository productRepository
    )
    {
        _logger = logger;
        _cacheService = cacheService;
        _productRepository = productRepository;
    }

    public Product CreateProduct(Product product)
    {
        return _productRepository.Add(product);
    }

    public async Task<GetAllProductsResultForCustomer> GetAllProductsForCustomerAsync(GetAllProductsQueryForCustomer query)
    {
        var cacheKey = ProductCacheKeys.GetAllProductsKey(query.GetContentHash());
        var cachedResult = await _cacheService.GetAsync<GetAllProductsResultForCustomer>(cacheKey);

        if (cachedResult is not null)
        {
            return cachedResult;
        }

        var products = await _productRepository.FindAllWithFilterForCustomerAsync(query);
        var result = GetAllProductsResultForCustomer.FromFilterResult(products);

        await _cacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(ProductCacheKeys.GetAllProductsTtlMinutes)
        );

        return result;
    }

    public async Task<Product> GetProductByIdAsync(Guid id)
    {
        var cacheKey = ProductCacheKeys.GetProductKey(id);
        var cachedProduct = await _cacheService.GetAsync<Product>(cacheKey);

        if (cachedProduct is not null)
        {
            return cachedProduct;
        }

        var product = await _productRepository.FindByIdAsync(id)
            ?? throw new ResourceNotFoundException(
                ProductErrorCodes.NotFoundById,
                $"Product with Id = {id} was not found."
            );

        await _cacheService.SetAsync(
            cacheKey,
            product,
            TimeSpan.FromMinutes(ProductCacheKeys.GetProductTtlMinutes)
        );

        return product;
    }

    public async Task<Product> GetProductByIdForUpdateAsync(Guid id)
    {
        return await _productRepository.FindByIdAsync(id, true)
            ?? throw new ResourceNotFoundException(
                ProductErrorCodes.NotFoundById,
                $"Product with Id = {id} was not found."
            );
    }
}
