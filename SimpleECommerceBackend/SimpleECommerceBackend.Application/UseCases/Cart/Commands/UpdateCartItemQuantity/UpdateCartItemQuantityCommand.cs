using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Cart.Commands.UpdateCartItemQuantity;

public record UpdateCartItemQuantityCommand(
    Guid UserId,
    Guid ProductId,
    int NewQuantity
) : IRequest;

[AutoConstructor]
public partial class UpdateCartItemQuantityHandler : IRequestHandler<UpdateCartItemQuantityCommand>
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _cartItemRepository.FindByIdAsync(request.ProductId, request.UserId);
        if (cartItem is null)
            throw new KeyNotFoundException($"Item not found in cart");

        if (request.NewQuantity <= 0)
        {
            // If quantity is 0 or less, maybe remove? 
            // Command implies "Update", usually to specific value. 
            // Entity throws if <= 0.
            // Let's assume remove if 0, or throw.
            // If 0, call remove repo.
            if (request.NewQuantity == 0)
            {
                _cartItemRepository.Delete(cartItem);
            }
            else
            {
                 throw new ArgumentException("Quantity must be positive");
            }
        }
        else
        {
            // Check inventory
            var inventory = await _inventoryRepository.FindByProductIdAsync(request.ProductId);
            if (inventory is null || inventory.AvailableQuantity < request.NewQuantity) // Check against total logic? 
            // Wait, Inventory AvailableQuantity is what's in warehouse. 
            // Logic: Is AvailableQuantity enough for the NEW quantity?
            // If user holds 5, inventory has 10. User wants 8. 
            // Does user "hold" inventory? No, cart doesn't reserve. 
            // So we check if Inventory.Available >= NewQuantity.
            // Correct.
            if (inventory is null || inventory.AvailableQuantity < request.NewQuantity)
                 throw new Exception("Insufficient stock");

            cartItem.SetQuantity(request.NewQuantity);
            _cartItemRepository.Update(cartItem);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
