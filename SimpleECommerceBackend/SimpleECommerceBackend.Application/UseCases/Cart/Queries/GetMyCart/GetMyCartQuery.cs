using MediatR;
using SimpleECommerceBackend.Application.UseCases.Cart.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Cart.Queries.GetMyCart;

public record GetMyCartQuery(Guid UserId) : IRequest<CartDto?>;

[AutoConstructor]
public partial class GetMyCartHandler : IRequestHandler<GetMyCartQuery, CartDto?>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;

    public async Task<CartDto?> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.FindByCustomerIdAsync(request.UserId);
        
        // If cart doesn't exist, return empty dto or null? 
        // Usually return empty cart for valid user.
        // For now returning null to indicate "no active cart found" or empty.
        // Let's return new CartDto with empty items if not found, or maybe null.
        // Implementation plan says "Returns Cart object".
        if (cart is null)
        {
            return new CartDto(request.UserId, new List<CartItemDto>(), 0, "VND");
        }

        var cartItems = await _cartItemRepository.FindByCustomerIdAsync(request.UserId);
        var itemDtos = new List<CartItemDto>();
        decimal totalAmount = 0;
        string currency = "VND"; // Default

        foreach (var item in cartItems)
        {
            var product = await _productRepository.FindByIdAsync(item.ProductId);
            if (product != null) 
            {
                // Should check if product is valid/active? Yes.
                // Assuming FindByIdAsync filters deleted.
                
                // Get main image
                var images = await _productImageRepository.FindByProductIdAsync(item.ProductId);
                var mainImage = images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.ImageUrl ?? "";

                var lineTotal = item.Quantity * product.CurrentPrice.Amount;
                totalAmount += lineTotal;
                currency = product.CurrentPrice.Currency; // Assuming all same currency

                itemDtos.Add(new CartItemDto(
                    item.ProductId,
                    product.Name,
                    mainImage,
                    product.CurrentPrice.Amount,
                    product.CurrentPrice.Currency,
                    item.Quantity,
                    lineTotal
                ));
            }
        }

        return new CartDto(request.UserId, itemDtos, totalAmount, currency);
    }
}
