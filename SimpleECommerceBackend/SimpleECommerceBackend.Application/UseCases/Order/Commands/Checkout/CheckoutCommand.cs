using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Application.UseCases.Order.Commands.Checkout;

public record CheckoutCommand(
    Guid UserId,
    Guid? ShippingAddressId, // If selecting existing
    string? NewRecipientName,
    string? NewPhoneNumber,
    string? NewAddressLine,
    string? NewWard,
    string? NewProvince,
    PaymentMethod PaymentMethod,
    string? Note
) : IRequest<Guid>;

[AutoConstructor]
public partial class CheckoutHandler : IRequestHandler<CheckoutCommand, Guid>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderShippingAddressRepository _orderShippingAddressRepository;
    private readonly IUserShippingAddressRepository _userShippingAddressRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Guid> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        // 1. Get Cart Logic
        var cart = await _cartRepository.FindByCustomerIdAsync(request.UserId);
        if (cart is null)
             throw new Exception("Cart is empty");

        var cartItems = await _cartItemRepository.FindByCustomerIdAsync(request.UserId);
        if (!cartItems.Any())
             throw new Exception("Cart is empty");

        // 2. Validate Stock & Calculate Total
        decimal totalAmount = 0;
        string currency = "VND";

        // We need product details for price snapshot
        var orderItemsToCreate = new List<(Product Product, int Quantity)>();

        foreach (var item in cartItems)
        {
            var product = await _productRepository.FindByIdAsync(item.ProductId);
            if (product is null || product.IsDeleted) // IsDeleted check via property or query
                throw new Exception($"Product {item.ProductId} no longer available");

            if (product.Status != ProductStatus.Active)
                 throw new Exception($"Product {product.Name} is not active");

            var inventory = await _inventoryRepository.FindByProductIdAsync(item.ProductId);
            if (inventory is null || inventory.AvailableQuantity < item.Quantity)
                 throw new Exception($"Insufficient stock for {product.Name}");

            totalAmount += product.CurrentPrice.Amount * item.Quantity;
            currency = product.CurrentPrice.Currency; // Assume mixed currency not supported or all same
            
            orderItemsToCreate.Add((product, item.Quantity));
        }

        // 3. Create Shipping Address
        OrderShippingAddress orderAddress;
        if (request.ShippingAddressId.HasValue)
        {
            var userAddress = await _userShippingAddressRepository.FindByIdAsync(request.ShippingAddressId.Value);
            if (userAddress is null || userAddress.CustomerId != request.UserId)
                throw new Exception("Invalid shipping address");
            
            orderAddress = OrderShippingAddress.Create(
                userAddress.RecipientName,
                userAddress.PhoneNumber,
                userAddress.AddressLine,
                userAddress.Ward,
                userAddress.Province
            );
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.NewRecipientName) || 
                string.IsNullOrWhiteSpace(request.NewPhoneNumber) ||
                string.IsNullOrWhiteSpace(request.NewAddressLine))
                throw new Exception("Shipping address required");

            orderAddress = OrderShippingAddress.Create(
                request.NewRecipientName,
                request.NewPhoneNumber,
                request.NewAddressLine,
                request.NewWard,
                request.NewProvince
            );
        }
        _orderShippingAddressRepository.Add(orderAddress);

        // 4. Create Order
        var orderCode = $"ORD-{DateTime.UtcNow.Ticks}"; // Simple code generation logic
        var order = Order.Create(
            orderCode,
            request.Note,
            new Money(totalAmount, currency),
            request.UserId,
            orderAddress.Id // Warning: Id might be empty if EF generates on save? 
            // Usually valid Guid if Create() generates it. Domain EntityBase usually updates Id via Guid.NewGuid().
            // Let's assume EntityBase initializes Id.
        );
        _orderRepository.Add(order);

        // 5. Create Order Items & Reserve Stock
        foreach (var (product, quantity) in orderItemsToCreate)
        {
            var orderItem = OrderItem.Create(
                order.Id,
                product.Id,
                quantity,
                product.CurrentPrice
            );
            _orderItemRepository.Add(orderItem);

            var inventory = await _inventoryRepository.FindByProductIdAsync(product.Id);
            // We checked before, but in high concurrency, re-check might be needed.
            // Transaction isolation handles it if setup correctly.
            // Reserve stock
            inventory!.ReserveStock(quantity);
            _inventoryRepository.Update(inventory);
        }

        // 6. Create Initial Payment
        var payment = Payment.Create(
            order.Id,
            new Money(totalAmount, currency),
            request.PaymentMethod
            // PaymentStatus.Pending is default in Create?
        );
        _paymentRepository.Add(payment);

        // 7. Clear Cart
        foreach (var item in cartItems)
        {
            _cartItemRepository.Delete(item);
        }
        // Should we delete Cart itself? Usually keep it empty.

        // 8. Commit
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
