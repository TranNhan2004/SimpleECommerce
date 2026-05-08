using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Models.Products;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Services;

public class ProductService : ServiceBase, IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductPriceRepository _productPriceRepository;

    public ProductService(
        ICacheService cacheService,
        IProductRepository productRepository,
        IProductPriceRepository productPriceRepository
    ) : base(cacheService)
    {
        _productRepository = productRepository;
        _productPriceRepository = productPriceRepository;
    }

    public Product CreateProduct(Product product)
    {
        var createdProduct = _productRepository.Add(product);
        _productPriceRepository.Add(new ProductPrice
        {
            ProductId = createdProduct.Id,
            Money = product.CurrentPrice,
            EffectiveFrom = DateTimeOffset.UtcNow
        });

        return createdProduct;
    }

    public async Task<GetAllProductsResultForCustomer> GetAllProductsForCustomerAsync(GetAllProductsQueryForCustomer query)
    {
        var cacheKey = ProductCacheKeys.GetAllProductsKey(query.GetContentHash());
        var cachedResult = await CacheService.GetAsync<GetAllProductsResultForCustomer>(cacheKey);

        if (cachedResult is not null)
        {
            return cachedResult;
        }

        var products = await _productRepository.FindAllWithFilterForCustomerAsync(query);
        var result = GetAllProductsResultForCustomer.FromFilterResult(products);

        await CacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(ProductCacheKeys.GetAllProductsTtlMinutes)
        );

        return result;
    }

    public async Task<Product> GetProductByIdAsync(Guid id)
    {
        var cacheKey = ProductCacheKeys.GetProductKey(id);
        var cachedProduct = await CacheService.GetAsync<Product>(cacheKey);

        if (cachedProduct is not null)
        {
            return cachedProduct;
        }

        var product = await _productRepository.FindByIdAsync(id)
            ?? throw new ResourceNotFoundException(
                ProductErrorCodes.NotFoundById,
                $"Product with Id = {id} was not found."
            );

        await CacheService.SetAsync(
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