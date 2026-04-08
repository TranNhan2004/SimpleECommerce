using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class NotificationTests
{
    [Fact]
    public void Create_ShouldCreateNotification_WhenInputIsValid()
    {
        var notification = Notification.Create(Guid.NewGuid(), "  Order updated  ");

        notification.Message.Should().Be("Order updated");
        notification.IsRead.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenMessageIsBlank()
    {
        var action = () => Notification.Create(Guid.NewGuid(), " ");

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(NotificationErrorCode.MessageRequired);
    }

    [Fact]
    public void Create_ShouldThrowValidationException_WhenMessageExceedsMaxLength()
    {
        var message = new string('a', NotificationConstants.MessageMaxLength + 1);
        var action = () => Notification.Create(Guid.NewGuid(), message);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(NotificationErrorCode.MessageMaxLengthExceeded);
    }

    [Fact]
    public void MarkAsRead_ShouldThrowValidationException_WhenNotificationIsAlreadyRead()
    {
        var notification = Notification.Create(Guid.NewGuid(), "Order updated");
        notification.MarkAsRead();
        var action = () => notification.MarkAsRead();

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(NotificationErrorCode.AlreadyMarkedAsRead);
    }
}
