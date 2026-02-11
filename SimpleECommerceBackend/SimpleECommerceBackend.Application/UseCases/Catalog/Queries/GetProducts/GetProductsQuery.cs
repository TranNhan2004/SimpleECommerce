using MediatR;
using SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Catalog.Queries.GetProducts;

public record GetProductsQuery : IRequest<IEnumerable<ProductDto>>; // TODO: Add pagination/filtering inputs

[AutoConstructor]
public partial class GetProductsHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    // For listing, we might skip images/inventory for performance or fetch efficiently 
    // Simplified implementation for now

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.FindAllAsync();

        // Note: This mapping is simple but N+1 if we fetch details separately. 
        // Ideally Repository should return a projection or we accept missing details in list view.
        // For now, I'll map what's available in Product entity.

        var dtos = new List<ProductDto>();

        foreach (var product in products)
        {
            // Simplified: Not fetching images/inventory for list to avoid N+1
            // In real world, use a specific View Model or include essential data in query
            dtos.Add(new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.CurrentPrice.Amount,
                product.CurrentPrice.Currency,
                product.Status.ToString(),
                product.CategoryId,
                product.Category?.Name ?? string.Empty,
                product.SellerId,
                0, // Inventory not loaded
                new List<ProductImageDto>(), // Images not loaded
                product.CreatedAt,
                product.UpdatedAt
            ));
        }

        return dtos;
    }
}
