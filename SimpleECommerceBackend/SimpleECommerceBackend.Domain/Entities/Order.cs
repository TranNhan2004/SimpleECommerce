using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities;

public class Order : Entity, ICreatedTrackable, IUpdatedTrackable
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
        SetId(Guid.NewGuid());
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

    public string Code { get; private set; } = null!;
    public string? Note { get; private set; }
    public Money ShippingFee { get; private set; }
    public Money TotalPrice { get; private set; }
    public OrderStatus Status { get; private set; }

    public Guid CustomerId { get; private set; }
    public UserProfile? Customer { get; private set; }
    public Guid SellerId { get; private set; }
    public UserProfile? Seller { get; private set; }

    public string ShopName { get; private set; } = null!;
    public Address WarehouseAddress { get; private set; }

    public string RecipientName { get; private set; } = null!;
    public string RecipientPhoneNumber { get; private set; } = null!;
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
            throw new BusinessException("Order code is required");

        var trimmedCode = code.Trim();

        if (trimmedCode.Length > OrderConstants.CodeMaxLength)
            throw new BusinessException($"Order code cannot exceed {OrderConstants.CodeMaxLength} characters");

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
            throw new BusinessException("Note is not blank");

        var trimmedNote = note.Trim();

        if (trimmedNote.Length > OrderConstants.NoteMaxLength)
            throw new BusinessException($"Note cannot exceed {OrderConstants.NoteMaxLength} characters");

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
            throw new BusinessException("Only pending payment orders can be picked up");

        SetStatus(OrderStatus.ReadyToPickup);
    }

    public void Ship()
    {
        if (Status != OrderStatus.ReadyToPickup)
            throw new BusinessException("Only picked up orders can be shipped");

        SetStatus(OrderStatus.Shipped);
    }

    public void AwaitConfirmation()
    {
        if (Status != OrderStatus.Shipped)
            throw new BusinessException("Only shipped orders can be awaited confirmation");

        SetStatus(OrderStatus.AwaitingConfirmation);
    }

    public void Deliver()
    {
        if (Status != OrderStatus.AwaitingConfirmation)
            throw new BusinessException("Only awaiting confirmation orders can be delivered");

        SetStatus(OrderStatus.Delivered);
    }

    public void Cancel()
    {
        if (Status != OrderStatus.PendingPayment && Status != OrderStatus.ReadyToPickup)
            throw new BusinessException("Only pending payment or picked up orders can be cancelled");

        SetStatus(OrderStatus.Cancelled);
    }

    public void Return()
    {
        if (Status != OrderStatus.AwaitingConfirmation)
            throw new BusinessException("Only awaiting confirmation orders can be returned");

        SetStatus(OrderStatus.Returned);
    }

    public void Expire()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new BusinessException("Only pending payment orders can be expired");

        SetStatus(OrderStatus.Expired);
        ExpiredAt = DateTimeOffset.UtcNow.AddDays(1);
    }

    public void SetShopName(string shopName)
    {
        if (string.IsNullOrWhiteSpace(shopName))
            throw new BusinessException("Shop name is required");

        var trimmedName = shopName.Trim();

        if (trimmedName.Length > SellerShopConstants.NameMaxLength)
            throw new BusinessException(
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
            throw new BusinessException("Recipient name is required");

        var trimmedName = recipientName.Trim();

        if (trimmedName.Length > ShippingAddressConstants.RecipientNameMaxLength)
            throw new BusinessException(
                $"Recipient name cannot exceed {ShippingAddressConstants.RecipientNameMaxLength} characters");

        RecipientName = trimmedName;
    }

    public void SetRecipientPhoneNumber(string recipientPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(recipientPhoneNumber))
            throw new BusinessException("Recipient phone number is required");

        var trimmedRecipientPhoneNumber = recipientPhoneNumber.Trim();

        if (trimmedRecipientPhoneNumber.Length > CommonConstants.PhoneNumberMaxLength)
            throw new BusinessException(
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
            throw new BusinessException("Customer is required");

        CustomerId = customerId;
    }

    public void SetSellerId(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
            throw new BusinessException("Seller is required");

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