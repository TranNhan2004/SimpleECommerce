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

    public Order()
    {
    }

    // private Order(
    //     string code,
    //     string? note,
    //     Money shippingFee,
    //     OrderStatus status,
    //     string shopName,
    //     Address warehouseAddress,
    //     string recipientName,
    //     string recipientPhoneNumber,
    //     Address recipientAddress,
    //     Guid customerId,
    //     Guid sellerId
    // )
    // {
    //     Id = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();
    //     Code = code;
    //     Note = note;
    //     ShippingFee = shippingFee;
    //     TotalPrice = shippingFee;
    //     Status = status;
    //     ShopName = shopName;
    //     WarehouseAddress = warehouseAddress;
    //     RecipientName = recipientName;
    //     RecipientPhoneNumber = recipientPhoneNumber;
    //     RecipientAddress = recipientAddress;
    //     CustomerId = customerId;
    //     SellerId = sellerId;
    // }

    private string _code = null!;
    private string? _note;
    private Money _shippingFee;
    private Money _totalPrice;
    private OrderStatus _status;
    private Guid _customerId;
    private Guid _sellerId;
    private string _shopName = null!;
    private Address _warehouseAddress;
    private string _recipientName = null!;
    private string _recipientPhoneNumber = null!;
    private Address _recipientAddress;

    public string Code
    {
        get => _code;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    OrderErrorCodes.CodeRequired,
                    "Order code is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "OrderCode"
                    }
                );

            var trimmedCode = value.Trim();

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

            _code = trimmedCode;
        }
    }

    public string? Note
    {
        get => _note;
        set
        {
            if (value is null)
            {
                _note = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    OrderErrorCodes.NoteMustNotBeBlank,
                    "Note is not blank",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Note"
                    }
                );

            var trimmedNote = value.Trim();

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

            _note = trimmedNote;
        }
    }

    public Money ShippingFee
    {
        get => _shippingFee;
        set => _shippingFee = value;
    }

    public Money TotalPrice
    {
        get => _totalPrice;
        set => _totalPrice = value;
    }

    public OrderStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public Guid CustomerId
    {
        get => _customerId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    OrderErrorCodes.CustomerRequired,
                    "Customer is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Customer"
                    }
                );

            _customerId = value;
        }
    }

    public UserProfile? Customer { get; private set; }
    public Guid SellerId
    {
        get => _sellerId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    OrderErrorCodes.SellerRequired,
                    "Seller is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Seller"
                    }
                );

            _sellerId = value;
        }
    }

    public UserProfile? Seller { get; private set; }

    public string ShopName
    {
        get => _shopName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    OrderErrorCodes.ShopNameRequired,
                    "Shop name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ShopName"
                    }
                );

            var trimmedName = value.Trim();

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

            _shopName = trimmedName;
        }
    }

    public Address WarehouseAddress
    {
        get => _warehouseAddress;
        set => _warehouseAddress = value;
    }

    public string RecipientName
    {
        get => _recipientName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    OrderErrorCodes.RecipientNameRequired,
                    "Recipient name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "RecipientName"
                    }
                );

            var trimmedName = value.Trim();

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

            _recipientName = trimmedName;
        }
    }

    public string RecipientPhoneNumber
    {
        get => _recipientPhoneNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    OrderErrorCodes.RecipientPhoneNumberRequired,
                    "Recipient phone number is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "RecipientPhoneNumber"
                    }
                );

            var trimmedRecipientPhoneNumber = value.Trim();

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

            _recipientPhoneNumber = trimmedRecipientPhoneNumber;
        }
    }

    public Address RecipientAddress
    {
        get => _recipientAddress;
        set => _recipientAddress = value;
    }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    public DateTimeOffset? ExpiredAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

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

        Status = OrderStatus.ReadyToPickup;
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

        Status = OrderStatus.Shipped;
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

        Status = OrderStatus.AwaitingConfirmation;
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

        Status = OrderStatus.Delivered;
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

        Status = OrderStatus.Cancelled;
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

        Status = OrderStatus.Returned;
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

        Status = OrderStatus.Expired;
        ExpiredAt = DateTimeOffset.UtcNow.AddDays(1);
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
