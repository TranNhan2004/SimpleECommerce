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
                OrderErrorCodes.CodeRequired,
                "Order code is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "OrderCode"
                }
            );

        var trimmedCode = code.Trim();

        if (trimmedCode.Length > OrderValidationRules.CodeMaxLength)
            throw new ValidationException(
                OrderErrorCodes.CodeMaxLengthExceeded,
                $"Order code cannot exceed {OrderValidationRules.CodeMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "OrderCode",
                    ["max"] = OrderValidationRules.CodeMaxLength
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
                OrderErrorCodes.NoteMustNotBeBlank,
                "Note is not blank",
                new Dictionary<string, object?>
                {
                    ["field"] = "Note"
                }
            );

        var trimmedNote = note.Trim();

        if (trimmedNote.Length > OrderValidationRules.NoteMaxLength)
            throw new ValidationException(
                OrderErrorCodes.NoteMaxLengthExceeded,
                $"Note cannot exceed {OrderValidationRules.NoteMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Note",
                    ["max"] = OrderValidationRules.NoteMaxLength
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
                OrderErrorCodes.PickupNotAllowed,
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
                OrderErrorCodes.ShipNotAllowed,
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
                OrderErrorCodes.AwaitConfirmationNotAllowed,
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
                OrderErrorCodes.DeliverNotAllowed,
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
                OrderErrorCodes.CancelNotAllowed,
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
                OrderErrorCodes.ReturnNotAllowed,
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
                OrderErrorCodes.ExpireNotAllowed,
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
                OrderErrorCodes.ShopNameRequired,
                "Shop name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "ShopName"
                }
            );

        var trimmedName = shopName.Trim();

        if (trimmedName.Length > SellerShopValidationRules.NameMaxLength)
            throw new ValidationException(
                OrderErrorCodes.ShopNameMaxLengthExceeded,
                $"Seller shop name cannot exceed {SellerShopValidationRules.NameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "ShopName",
                    ["max"] = SellerShopValidationRules.NameMaxLength
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
                OrderErrorCodes.RecipientNameRequired,
                "Recipient name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientName"
                }
            );

        var trimmedName = recipientName.Trim();

        if (trimmedName.Length > ShippingAddressValidationRules.RecipientNameMaxLength)
            throw new ValidationException(
                OrderErrorCodes.RecipientNameMaxLengthExceeded,
                $"Recipient name cannot exceed {ShippingAddressValidationRules.RecipientNameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientName",
                    ["max"] = ShippingAddressValidationRules.RecipientNameMaxLength
                }
            );

        RecipientName = trimmedName;
    }

    public void SetRecipientPhoneNumber(string recipientPhoneNumber)
    {
        if (string.IsNullOrWhiteSpace(recipientPhoneNumber))
            throw new ValidationException(
                OrderErrorCodes.RecipientPhoneNumberRequired,
                "Recipient phone number is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientPhoneNumber"
                }
            );

        var trimmedRecipientPhoneNumber = recipientPhoneNumber.Trim();

        if (trimmedRecipientPhoneNumber.Length > CommonValidationRules.PhoneNumberMaxLength)
            throw new ValidationException(
                OrderErrorCodes.RecipientPhoneNumberMaxLengthExceeded,
                $"Recipient phone number cannot exceed {CommonValidationRules.PhoneNumberMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "RecipientPhoneNumber",
                    ["max"] = CommonValidationRules.PhoneNumberMaxLength
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
                OrderErrorCodes.CustomerRequired,
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
                OrderErrorCodes.SellerRequired,
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
