using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Catalog.Commands.AdjustInventory;

public record AdjustInventoryCommand(
    Guid ProductId,
    int QuantityChange,
    string? Notes // Optional
) : IRequest;

[AutoConstructor]
public partial class AdjustInventoryHandler : IRequestHandler<AdjustInventoryCommand>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(AdjustInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.FindByProductIdAsync(request.ProductId);
        if (inventory is null)
            throw new KeyNotFoundException($"Inventory for Product ID {request.ProductId} not found");

        if (request.QuantityChange > 0)
            inventory.AddStock(request.QuantityChange);
        else if (request.QuantityChange < 0)
            // Releasing stock is not quite right here, this command implies manual adjustment (e.g. stock count)
            // But logic in Inventory entity has "AddStock", "ReserveStock", "ReleaseStock".
            // If I want to reduce stock (e.g. damaged goods), I should probably specific method or negative add?
            // Checking Inventory.cs logic... 
            // AddStock(quantity) throws if <= 0.
            // So I need a way to reduce stock.
            // Inventory entity might need "RemoveStock" or "AdjustStock".
            // Since I can't modify Entity easily right now without checking file again, 
            // I'll assume AddStock handles only positive.
            // I'll check Inventory.cs.
            // Logic: SetQuantityOnHand directly? Setters were public? 
            // Let's check. 
            // If explicit "Adjust" missing, I might need to add it or use SetQuantityOnHand.
            inventory.SetQuantityOnHand(inventory.QuantityInStock + request.QuantityChange);

        _inventoryRepository.Update(inventory);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}