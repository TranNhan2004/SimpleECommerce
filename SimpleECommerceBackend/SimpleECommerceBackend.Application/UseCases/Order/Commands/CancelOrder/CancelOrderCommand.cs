using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Order.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest;

[AutoConstructor]
public partial class CancelOrderHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IPaymentRepository _paymentRepository; // To refund if paid
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.FindByIdAsync(request.OrderId);
        if (order is null)
            throw new KeyNotFoundException($"Order {request.OrderId} not found");

        // Logic handled in Domain method 'Cancel()'.
        // Validates status (can trigger refund?)
        order.Cancel();

        // Release Inventory
        // Need to loop items and ReleaseStock.
        // Or if 'Processing', 'Confirmed' -> stock is Reserved.
        // If 'Shipped', usually can't cancel (Domain checks this).
        
        // IOrderRepository FindByIdAsync usually includes Items? I assumed yes in GetOrderDetails. 
        // If not, explicit load needed.
        // I'll assume loaded or load them.
        IEnumerable<Domain.Entities.Business.OrderItem> items = order.OrderItems;
        if (!items.Any())
        {
            // Try to load if lazy/not included
            items = await _orderItemRepository.FindByOrderIdAsync(request.OrderId);
        }

        foreach (var item in items)
        {
            var inventory = await _inventoryRepository.FindByProductIdAsync(item.ProductId);
            if (inventory != null)
            {
                // Release reserved stock
                inventory.ReleaseStock(item.Quantity);
                _inventoryRepository.Update(inventory);
            }
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
