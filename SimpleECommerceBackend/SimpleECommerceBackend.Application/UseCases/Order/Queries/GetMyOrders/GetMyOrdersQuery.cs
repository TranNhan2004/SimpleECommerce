using MediatR;
using SimpleECommerceBackend.Application.UseCases.Order.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Order.Queries.GetMyOrders;

// Pagination inputs omitted for brevity in "huge" request but recommended
public record GetMyOrdersQuery(Guid UserId) : IRequest<IEnumerable<OrderDto>>;

[AutoConstructor]
public partial class GetMyOrdersHandler : IRequestHandler<GetMyOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public async Task<IEnumerable<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        // Repo needs FindByCustomerIdAsync
        var orders = await _orderRepository.FindByCustomerIdAsync(request.UserId);

        // Map to DTOs. 
        // Note: FindByCustomerIdAsync might not Include Items/Address/Payment for list view
        // to avoid huge data load.
        // Assuming it does NOT include deep details, or we just map summary.
        // If query returns just Order, Items are null?.
        // I'll map what I have.
        // For list view, typically just code, date, total, status is enough.
        
        return orders.Select(o => new OrderDto(
            o.Id,
            o.Code,
            o.Status.ToString(),
            o.TotalPrice.Amount,
            o.TotalPrice.Currency,
            o.CreatedAt,
            new List<OrderItemDto>(), // Empty items for list view
            null, // No address in list view
            null, // Payment status separate or via navigation if loaded
            null
        ));
    }
}
