using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Order : EntityBase, ICreatedTime, IUpdatedTime
{
    private readonly List<OrderItem> _orderItems = [];

    private Order()
    {
    }

    private Order(
        string code,
        string? note,
        Money shippingFee,
        OrderStatus status,
        string shopName,
        Address warehouseAddress,
        string recipientName,
        string recipientPhoneNumber,
        Address recipientAddress,
        Guid customerId,
        Guid sellerId
    )
    {
        SetCode(code);
        SetNote(note);
        SetShoppingFee(shippingFee);
        SetTotalPrice(shippingFee);
        SetStatus(status);
        SetShopName(shopName);
        SetWarehouseAddress(warehouseAddress);
        SetRecipientName(recipientName);
        SetRecipientPhoneNumber(recipientPhoneNumber);
        SetRecipientAddress(recipientAddress);
        SetCustomerId(customerId);
        SetSellerId(sellerId);
    }

    public string Code { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public Money ShippingFee { get; private set; }
    public Money TotalPrice { get; private set; }
    public OrderStatus Status { get; private set; }

    public Guid CustomerId { get; private set; }
    public UserProfile? Customer { get; private set; }
    public Guid SellerId { get; private set; }
    public UserProfile? Seller { get; private set; }

    public string ShopName { get; private set; } = string.Empty;
    public Address WarehouseAddress { get; private set; }

    public string RecipientName { get; private set; } = string.Empty;
    public string RecipientPhoneNumber { get; private set; } = string.Empty;
    public Address RecipientAddress { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    public DateTimeOffset? ExpiredAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }


    public static Order Create(
        string code,
        string? note,
        Money shippingFee,
        OrderStatus status,
        string shopName,
        Address warehouseAddress,
        string recipientName,
        string recipientPhoneNumber,
        Address recipientAddress,
        Guid customerId,
        Guid sellerId
    )
    {
        return new Order(
            code,
            note,
            shippingFee,
            status,
            shopName,
            warehouseAddress,
            recipientName,
            recipientPhoneNumber,
            recipientAddress,
            customerId,
            sellerId
        );
    }

    public void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Order code is required");

        var trimmedCode = code.Trim();

        if (trimmedCode.Length > OrderConstants.CodeMaxLength)
            throw new DomainException($"Order code cannot exceed {OrderConstants.CodeMaxLength} characters");

        Code = trimmedCode;
    }

    public void SetNote(string? note)
    {
        if (note is null)
        {
            Note = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(note))
            throw new DomainException("Note is not blank");

        var trimmedNote = note.Trim();

        if (trimmedNote.Length > OrderConstants.NoteMaxLength)
            throw new DomainException($"Note cannot exceed {OrderConstants.NoteMaxLength} characters");

        Note = trimmedNote;
    }

    public void SetShoppingFee(Money shippingFee)
    {
        ShippingFee = shippingFee;
    }

    private void SetTotalPrice(Money totalPrice)
    {
        TotalPrice = totalPrice;
    }

    private void SetStatus(OrderStatus status)
    {
        Status = status;
    }

    public void Pickup()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new DomainException("Only pending payment orders can be picked up");

        SetStatus(OrderStatus.ReadyToPickup);
    }

    public void Ship()
    {
        if (Status != OrderStatus.ReadyToPickup)
            throw new DomainException("Only picked up orders can be shipped");

        SetStatus(OrderStatus.Shipped);
    }

    public void AwaitConfirmation()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainException("Only shipped orders can be awaited confirmation");

        SetStatus(OrderStatus.AwaitingConfirmation);
    }

    public void Deliver()
    {
        if (Status != OrderStatus.AwaitingConfirmation)
            throw new DomainException("Only awaiting confirmation orders can be delivered");

        SetStatus(OrderStatus.Delivered);
    }

    public void Cancel()
    {
        if (Status != OrderStatus.PendingPayment && Status != OrderStatus.ReadyToPickup)
            throw new DomainException("Only pending payment or picked up orders can be cancelled");

        SetStatus(OrderStatus.Cancelled);
    }

    public void Return()
    {
        if (Status != OrderStatus.AwaitingConfirmation)
            throw new DomainException("Only awaiting confirmation orders can be returned");

        SetStatus(OrderStatus.Returned);
    }

    public void Expire()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new DomainException("Only pending payment orders can be expired");

        SetStatus(OrderStatus.Expired);
        ExpiredAt = DateTimeOffset.UtcNow.AddDays(1);
    }

    public void SetShopName(string shopName)
    {
        if (string.IsNullOrWhiteSpace(shopName))
            throw new DomainException("Shop name is required");

        var trimmedName = shopName.Trim();

        if (trimmedName.Length > SellerShopConstants.NameMaxLength)
            throw new DomainException(
                $"Seller shop name cannot exceed {SellerShopConstants.NameMaxLength} characters");

        ShopName = shopName;
    }

    public void SetWarehouseAddress(Address warehouseAddress)
    {
        WarehouseAddress = warehouseAddress;
    }

    public void SetRecipientName(string recipientName)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
            throw new DomainException("Recipient name is required");

        var trimmedName = recipientName.Trim();

        if (trimmedName.Length > ShippingAddressConstants.RecipientNameMaxLength)
            throw new DomainException(
                $"Recipient name cannot exceed {ShippingAddressConstants.RecipientNameMaxLength} characters");

        RecipientName = trimmedName;
    }

    public void SetRecipientPhoneNumber(string recipientPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(recipientPhoneNumber))
            throw new DomainException("Recipient phone number is required");

        var trimmedRecipientPhoneNumber = recipientPhoneNumber.Trim();

        if (trimmedRecipientPhoneNumber.Length > CommonConstants.PhoneNumberMaxLength)
            throw new DomainException(
                $"Recipient phone number cannot exceed {CommonConstants.PhoneNumberMaxLength} characters");

        RecipientPhoneNumber = trimmedRecipientPhoneNumber;
    }

    public void SetRecipientAddress(Address recipientAddress)
    {
        RecipientAddress = recipientAddress;
    }

    public void SetCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Customer is required");

        CustomerId = customerId;
    }

    public void SetSellerId(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
            throw new DomainException("Seller is required");

        SellerId = sellerId;
    }

    public void CalculateTotalPrice(OrderItem item)
    {
        TotalPrice += item.CurrentPrice * item.Quantity;
    }

    public void AddItem(OrderItem item)
    {
        _orderItems.Add(item);
    }
}