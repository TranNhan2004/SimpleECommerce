using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Order.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest;

[AutoConstructor]
public partial class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.FindByIdAsync(request.OrderId);
        if (order is null)
            throw new KeyNotFoundException($"Order {request.OrderId} not found");

        // Use specific methods if possible, or SetStatus (private).
        // Domain has: Confirm(), StartProcessing(), Ship(), Deliver(), Return().
        // Handlers should map NewStatus to these methods to ensure transitions are valid. via Domain Logic?
        // Or unbox the logic?
        // Let's try to map.
        
        switch (request.NewStatus)
        {
            case OrderStatus.Confirmed:
                order.Confirm();
                break;
            case OrderStatus.Processing:
                order.StartProcessing();
                break;
            case OrderStatus.Shipped:
                order.Ship();
                break;
            case OrderStatus.Delivered:
                order.Deliver();
                break;
            case OrderStatus.Returned:
                order.Return();
                break;
            case OrderStatus.Cancelled:
                order.Cancel(); 
                // Note: Cancel logic also releases stock. 
                // If Admin sets Cancel via this command, we might miss stock release !
                // Better to use CancelOrderCommand for Cancellation.
                // Or duplicate logic?
                // I'll throw if they try to Cancel via UpdateStatus, forcing use of Cancel command.
                throw new InvalidOperationException("Use CancelOrderCommand to cancel orders to ensure inventory is released.");
            default:
                throw new ArgumentException("Invalid status transition via generic update");
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
