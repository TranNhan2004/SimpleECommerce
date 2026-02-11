using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Cart.Commands.AddToCart;

public record AddToCartCommand(
    Guid UserId,
    Guid ProductId,
    int Quantity
) : IRequest;

[AutoConstructor]
public partial class AddToCartHandler : IRequestHandler<AddToCartCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // 1. Check Inventory
        var inventory = await _inventoryRepository.FindByProductIdAsync(request.ProductId);
        if (inventory is null || inventory.AvailableQuantity < request.Quantity)
        {
            // Throw domain exception or custom application exception
             throw new Exception("Insufficient stock"); // Use specific exception in real app
        }

        // 2. Get or Create Cart
        var cart = await _cartRepository.FindByCustomerIdAsync(request.UserId);
        if (cart is null)
        {
            cart = Cart.Create(request.UserId);
            _cartRepository.Add(cart);
            // Need to save cart first to get foreign key constraints satisfied? 
            // EF Core usually handles this if added to context.
        }

        // 3. Check if Item exists
        var cartItem = await _cartItemRepository.FindByIdAsync(request.ProductId, request.UserId);
        if (cartItem is null)
        {
            cartItem = CartItem.Create(request.ProductId, request.UserId, request.Quantity);
            _cartItemRepository.Add(cartItem);
        }
        else
        {
            // Update quantity
            // Check max limit if needed. CartItem entity handles validation? 
            // CartItem.SetQuantity validates constraints.
            // Check inventory for TOTAL quantity (existing + new)
            if (inventory.AvailableQuantity < cartItem.Quantity + request.Quantity)
            {
                 throw new Exception("Insufficient stock for total quantity");
            }
            cartItem.IncreaseQuantity(request.Quantity);
            _cartItemRepository.Update(cartItem);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
