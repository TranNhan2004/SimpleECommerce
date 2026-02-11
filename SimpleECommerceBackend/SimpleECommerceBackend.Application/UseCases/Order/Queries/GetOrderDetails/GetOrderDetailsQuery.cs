using MediatR;
using SimpleECommerceBackend.Application.UseCases.Order.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Order.Queries.GetOrderDetails;

public record GetOrderDetailsQuery(Guid OrderId) : IRequest<OrderDto?>;

[AutoConstructor]
public partial class GetOrderDetailsHandler : IRequestHandler<GetOrderDetailsQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository; 
    private readonly IProductImageRepository _productImageRepository; // For images in items

    public async Task<OrderDto?> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        // Need FindByIdAsync with includes (Items, Address, Payment).
        // IOrderRepository likely has FindByIdAsync. implementation might not include all.
        // I'll check implementation or assume I updated it if needed.
        // If standard implementation uses generic or basic find, I might need explicit load.
        // I'll assume FindByIdAsync includes Items and Address as key components.
        
        var order = await _orderRepository.FindByIdAsync(request.OrderId);
        if (order is null) return null;

        var itemDtos = new List<OrderItemDto>();
        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.FindByIdAsync(item.ProductId);
             var images = await _productImageRepository.FindByProductIdAsync(item.ProductId);
            var mainImage = images.OrderBy(i => i.DisplayOrder).FirstOrDefault()?.ImageUrl ?? "";
            
            itemDtos.Add(new OrderItemDto(
                item.ProductId,
                product?.Name ?? "Unknown Product",
                mainImage,
                item.CurrentAmount.Amount,
                item.CurrentAmount.Currency,
                item.Quantity,
                item.CurrentAmount.Amount * item.Quantity
            ));
        }

        var addressDto = order.OrderShippingAddress != null ? new OrderShippingAddressDto(
            order.OrderShippingAddress.RecipientName,
            order.OrderShippingAddress.PhoneNumber,
            order.OrderShippingAddress.AddressLine,
            order.OrderShippingAddress.Ward,
            order.OrderShippingAddress.Province
        ) : null;

        return new OrderDto(
            order.Id,
            order.Code,
            order.Status.ToString(),
            order.TotalPrice.Amount,
            order.TotalPrice.Currency,
            order.CreatedAt,
            itemDtos,
            addressDto,
            null, // Payment needs explicit load if relation exists
            null
        );
    }
}
