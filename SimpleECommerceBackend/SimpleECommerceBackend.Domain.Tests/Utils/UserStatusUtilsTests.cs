using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Tests.Utils;

public class UserStatusUtilsTests
{
    [Theory]
    [InlineData("active", UserStatus.Active)]
    [InlineData("inactive", UserStatus.Inactive)]
    [InlineData("archived", UserStatus.Archived)]
    [InlineData(" Active ", UserStatus.Active)]
    public void Parse_ShouldReturnUserStatus_WhenInputIsValid(string input, UserStatus expected)
    {
        var result = UserStatusUtils.Parse(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void Parse_ShouldThrowValidationException_WhenInputIsInvalid()
    {
        var action = () => UserStatusUtils.Parse("unknown");

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserStatusErrorCode.InvalidUserStatus);
    }

    [Fact]
    public void ToName_ShouldReturnLowercaseName_WhenUserStatusIsSupported()
    {
        UserStatusUtils.ToName(UserStatus.Inactive).Should().Be("inactive");
    }
}
