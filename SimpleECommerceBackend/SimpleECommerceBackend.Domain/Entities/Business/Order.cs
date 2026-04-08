using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

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
            throw new ValidationException(
                OrderErrorCode.CodeRequired,
                "Order code is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "OrderCode"
                }
            );

        var trimmedCode = code.Trim();

        if (trimmedCode.Length > OrderConstants.CodeMaxLength)
            throw new ValidationException(
                OrderErrorCode.CodeMaxLengthExceeded,
                $"Order code cannot exceed {OrderConstants.CodeMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "OrderCode",
                    ["max"] = OrderConstants.CodeMaxLength
                }
            );

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
            throw new ValidationException(
                OrderErrorCode.NoteMustNotBeBlank,
                "Note is not blank",
                new Dictionary<string, object?>
                {
                    ["field"] = "Note"
                }
            );

        var trimmedNote = note.Trim();

        if (trimmedNote.Length > OrderConstants.NoteMaxLength)
            throw new ValidationException(
                OrderErrorCode.NoteMaxLengthExceeded,
                $"Note cannot exceed {OrderConstants.NoteMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Note",
                    ["max"] = OrderConstants.NoteMaxLength
                }
            );

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
            throw new ValidationException(
                OrderErrorCode.PickupNotAllowed,
                "Only pending payment orders can be picked up",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Pickup",
                    ["allowedStates"] = "pending payment"
                }
            );

        SetStatus(OrderStatus.ReadyToPickup);
    }

    public void Ship()
    {
        if (Status != OrderStatus.ReadyToPickup)
            throw new ValidationException(
                OrderErrorCode.ShipNotAllowed,
                "Only picked up orders can be shipped",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Ship",
                    ["allowedStates"] = "ready to pickup"
                }
            );

        SetStatus(OrderStatus.Shipped);
    }

    public void AwaitConfirmation()
    {
        if (Status != OrderStatus.Shipped)
            throw new ValidationException(
                OrderErrorCode.AwaitConfirmationNotAllowed,
                "Only shipped orders can be awaited confirmation",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Await confirmation",
                    ["allowedStates"] = "shipped"
                }
            );

        SetStatus(OrderStatus.AwaitingConfirmation);
    }

    public void Deliver()
    {
        if (Status != OrderStatus.AwaitingConfirmation)
            throw new ValidationException(
                OrderErrorCode.DeliverNotAllowed,
                "Only awaiting confirmation orders can be delivered",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Deliver",
                    ["allowedStates"] = "awaiting confirmation"
                }
            );

        SetStatus(OrderStatus.Delivered);
    }

    public void Cancel()
    {
        if (Status != OrderStatus.PendingPayment && Status != OrderStatus.ReadyToPickup)
            throw new ValidationException(
                OrderErrorCode.CancelNotAllowed,
                "Only pending payment or picked up orders can be cancelled",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Cancel",
                    ["allowedStates"] = "pending payment, ready to pickup"
                }
            );

        SetStatus(OrderStatus.Cancelled);
    }

    public void Return()
    {
        if (Status != OrderStatus.AwaitingConfirmation)
            throw new ValidationException(
                OrderErrorCode.ReturnNotAllowed,
                "Only awaiting confirmation orders can be returned",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Return",
                    ["allowedStates"] = "awaiting confirmation"
                }
            );

        SetStatus(OrderStatus.Returned);
    }

    public void Expire()
    {
        if (Status != OrderStatus.PendingPayment)
            throw new ValidationException(
                OrderErrorCode.ExpireNotAllowed,
                "Only pending payment orders can be expired",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Expire",
                    ["allowedStates"] = "pending payment"
                }
            );

        SetStatus(OrderStatus.Expired);
        ExpiredAt = DateTimeOffset.UtcNow.AddDays(1);
    }

    public void SetShopName(string shopName)
    {
        if (string.IsNullOrWhiteSpace(shopName))
            throw new ValidationException(
                OrderErrorCode.ShopNameRequired,
                "Shop name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "ShopName"
                }
            );

        var trimmedName = shopName.Trim();

        if (trimmedName.Length > SellerShopConstants.NameMaxLength)
            throw new ValidationException(
                OrderErrorCode.ShopNameMaxLengthExceeded,
                $"Seller shop name cannot exceed {SellerShopConstants.NameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "ShopName",
                    ["max"] = SellerShopConstants.NameMaxLength
                }
            );

        ShopName = trimmedName;
    }

    public void SetWarehouseAddress(Address warehouseAddress)
    {
        WarehouseAddress = warehouseAddress;
    }

    public void SetRecipientName(string recipientName)
    {
        if (string.IsNullOrWhiteSpace(recipientName))
            throw new ValidationException(
                OrderErrorCode.RecipientNameRequired,
                "Recipient name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientName"
                }
            );

        var trimmedName = recipientName.Trim();

        if (trimmedName.Length > ShippingAddressConstants.RecipientNameMaxLength)
            throw new ValidationException(
                OrderErrorCode.RecipientNameMaxLengthExceeded,
                $"Recipient name cannot exceed {ShippingAddressConstants.RecipientNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientName",
                    ["max"] = ShippingAddressConstants.RecipientNameMaxLength
                }
            );

        RecipientName = trimmedName;
    }

    public void SetRecipientPhoneNumber(string recipientPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(recipientPhoneNumber))
            throw new ValidationException(
                OrderErrorCode.RecipientPhoneNumberRequired,
                "Recipient phone number is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientPhoneNumber"
                }
            );

        var trimmedRecipientPhoneNumber = recipientPhoneNumber.Trim();

        if (trimmedRecipientPhoneNumber.Length > CommonConstants.PhoneNumberMaxLength)
            throw new ValidationException(
                OrderErrorCode.RecipientPhoneNumberMaxLengthExceeded,
                $"Recipient phone number cannot exceed {CommonConstants.PhoneNumberMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientPhoneNumber",
                    ["max"] = CommonConstants.PhoneNumberMaxLength
                }
            );

        RecipientPhoneNumber = trimmedRecipientPhoneNumber;
    }

    public void SetRecipientAddress(Address recipientAddress)
    {
        RecipientAddress = recipientAddress;
    }

    public void SetCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ValidationException(
                OrderErrorCode.CustomerRequired,
                "Customer is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Customer"
                }
            );

        CustomerId = customerId;
    }

    public void SetSellerId(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
            throw new ValidationException(
                OrderErrorCode.SellerRequired,
                "Seller is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Seller"
                }
            );

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
