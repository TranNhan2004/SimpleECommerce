using MediatR;
using SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Catalog.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

[AutoConstructor]
public partial class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id);

        if (product is null)
        {
            return null;
        }

        var images = await _productImageRepository.FindByProductIdAsync(request.Id);
        var inventory = await _inventoryRepository.FindByProductIdAsync(request.Id);

        var imageDtos = images.Select(i => new ProductImageDto(
            i.ProductId,
            i.ImageUrl,
            i.DisplayOrder,
            i.IsDisplayed,
            i.Description
        )).ToList();

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.CurrentPrice.Amount,
            product.CurrentPrice.Currency,
            product.Status.ToString(),
            product.CategoryId,
            product.Category?.Name ?? string.Empty,
            product.SellerId,
            inventory?.AvailableQuantity ?? 0,
            imageDtos,
            product.CreatedAt,
            product.UpdatedAt
        );
    }
}
