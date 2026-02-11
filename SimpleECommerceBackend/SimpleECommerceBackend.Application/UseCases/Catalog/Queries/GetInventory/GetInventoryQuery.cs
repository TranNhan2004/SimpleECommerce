using MediatR;
using SimpleECommerceBackend.Application.UseCases.Catalog.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Catalog.Queries.GetInventory;

public record GetInventoryQuery(Guid ProductId) : IRequest<InventoryDto?>;

[AutoConstructor]
public partial class GetInventoryHandler : IRequestHandler<GetInventoryQuery, InventoryDto?>
{
    private readonly IInventoryRepository _inventoryRepository;

    public async Task<InventoryDto?> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.FindByProductIdAsync(request.ProductId);
        if (inventory is null) return null;

        return new InventoryDto(
            inventory.ProductId,
            inventory.QuantityInStock,
            inventory.QuantityReserved,
            inventory.AvailableQuantity,
            inventory.Version,
            inventory.UpdatedAt ?? inventory.CreatedAt
        );
    }
}