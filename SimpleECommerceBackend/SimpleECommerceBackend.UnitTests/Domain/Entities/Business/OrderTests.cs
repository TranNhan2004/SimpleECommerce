// using FluentAssertions;
// using SimpleECommerceBackend.Domain.Constants.Business;
// using SimpleECommerceBackend.Domain.Entities.Business;
// using SimpleECommerceBackend.Domain.Enums;
// using SimpleECommerceBackend.Domain.Exceptions;
// using SimpleECommerceBackend.Domain.ValueObjects;
//
// namespace SimpleECommerceBackend.UnitTests.Domain.Entities.Business;
//
// public class OrderTests
// {
//     // ---------- Happy path ----------
//
//     [Fact]
//     public void Create_ShouldCreateOrder_WhenInputIsValid()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//
//         // Act
//         var order = Order.Create("ORD-001", "Please deliver fast", totalPrice, customerId, orderShippingAddressId);
//
//         // Assert
//         order.Should().NotBeNull();
//         order.Code.Should().Be("ORD-001");
//         order.Note.Should().Be("Please deliver fast");
//         order.TotalPrice.Should().Be(totalPrice);
//         order.CustomerId.Should().Be(customerId);
//         order.Status.Should().Be(OrderStatus.Pending);
//     }
//
//     [Fact]
//     public void Create_ShouldAllowNullNote()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//
//         // Act
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//
//         // Assert
//         order.Note.Should().BeNull();
//     }
//
//     // ---------- Code validation ----------
//
//     [Theory]
//     [InlineData("")]
//     [InlineData("   ")]
//     public void Create_ShouldThrow_WhenCodeIsEmptyOrWhitespace(string code)
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//
//         // Act
//         var act = () => Order.Create(code, null, totalPrice, customerId, orderShippingAddressId);
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be("Order code is required");
//     }
//
//     [Fact]
//     public void Create_ShouldThrow_WhenCodeExceedsMaxLength()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var code = new string('A', OrderConstants.CodeMaxLength + 1);
//
//         // Act
//         var act = () => Order.Create(code, null, totalPrice, customerId, orderShippingAddressId);
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be($"Order code cannot exceed {OrderConstants.CodeMaxLength} characters");
//     }
//
//     // ---------- Status transitions ----------
//
//     [Fact]
//     public void Confirm_ShouldChangeStatusToConfirmed_WhenStatusIsPending()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//
//         // Act
//         order.Confirm();
//
//         // Assert
//         order.Status.Should().Be(OrderStatus.Confirmed);
//     }
//
//     [Fact]
//     public void Confirm_ShouldThrow_WhenStatusIsNotPending()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//         order.Confirm();
//
//         // Act
//         var act = () => order.Confirm();
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be("Only pending orders can be confirmed");
//     }
//
//     [Fact]
//     public void StartProcessing_ShouldChangeStatusToProcessing_WhenStatusIsConfirmed()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//         order.Confirm();
//
//         // Act
//         order.StartProcessing();
//
//         // Assert
//         order.Status.Should().Be(OrderStatus.Processing);
//     }
//
//     [Fact]
//     public void Ship_ShouldChangeStatusToShipped_WhenStatusIsProcessing()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//         order.Confirm();
//         order.StartProcessing();
//
//         // Act
//         order.Ship();
//
//         // Assert
//         order.Status.Should().Be(OrderStatus.Shipped);
//     }
//
//     [Fact]
//     public void Deliver_ShouldChangeStatusToDelivered_WhenStatusIsShipped()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//         order.Confirm();
//         order.StartProcessing();
//         order.Ship();
//
//         // Act
//         order.Deliver();
//
//         // Assert
//         order.Status.Should().Be(OrderStatus.Delivered);
//     }
//
//     [Fact]
//     public void Cancel_ShouldChangeStatusToCancelled_WhenStatusIsPending()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//
//         // Act
//         order.Cancel();
//
//         // Assert
//         order.Status.Should().Be(OrderStatus.Cancelled);
//     }
//
//     [Fact]
//     public void Cancel_ShouldThrow_WhenStatusIsShipped()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//         order.Confirm();
//         order.StartProcessing();
//         order.Ship();
//
//         // Act
//         var act = () => order.Cancel();
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be("Cannot cancel shipped or delivered orders");
//     }
//
//     [Fact]
//     public void Return_ShouldChangeStatusToReturned_WhenStatusIsDelivered()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//         var order = Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//         order.Confirm();
//         order.StartProcessing();
//         order.Ship();
//         order.Deliver();
//
//         // Act
//         order.Return();
//
//         // Assert
//         order.Status.Should().Be(OrderStatus.Returned);
//     }
//
//     // ---------- Total price validation ----------
//     [Fact]
//     public void Money_ShouldThrow_WhenAmountIsNegative()
//     {
//         var act = () => new Money(-100, "VND");
//
//         act.Should()
//             .Throw<DomainException>()
//             .WithMessage("Money amount cannot be negative");
//     }
//
//     // ---------- CustomerId validation ----------
//     [Fact]
//     public void Create_ShouldThrow_WhenCustomerIdIsEmpty()
//     {
//         // Arrange
//         var customerId = Guid.Empty;
//         var orderShippingAddressId = Guid.Parse("7895564c-1b51-4a35-944b-be7750485fde");
//         var totalPrice = new Money(500000, "VND");
//
//         // Act
//         var act = () => Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be("Customer is required");
//     }
//
//
//     // ---------- OrderShippingAddressId validation ----------
//
//     [Fact]
//     public void Create_ShouldThrow_WhenOrderShippingAddressIdIsEmpty()
//     {
//         // Arrange
//         var customerId = Guid.Parse("6984453b-0a40-8324-833a-ad6649374fce");
//         var orderShippingAddressId = Guid.Empty;
//         var totalPrice = new Money(500000, "VND");
//
//         // Act
//         var act = () => Order.Create("ORD-001", null, totalPrice, customerId, orderShippingAddressId);
//
//         // Assert
//         var exception = act.Should().Throw<DomainException>().Which;
//         exception.Message.Should().Be("Order shipping address is required");
//     }
// }

