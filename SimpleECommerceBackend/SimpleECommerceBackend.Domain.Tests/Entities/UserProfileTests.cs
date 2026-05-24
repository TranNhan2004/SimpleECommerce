using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_ShouldCreateUser_WhenInputIsValid()
    {
        var user = CreateUser();

        user.Email.Should().Be("nhan@example.com");
        user.FirstName.Should().Be("Nhan");
        user.LastName.Should().Be("Nguyen");
        user.NickName.Should().Be("nhan");
        user.Sex.Should().Be(Sex.Male);
        user.Status.Should().Be(UserStatus.Active);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowValidationException_WhenFirstNameIsBlank(string firstName)
    {
        var action = () => new User
        {
            Id = Guid.NewGuid(),
            Email = "nhan@example.com",
            FirstName = firstName,
            LastName = "Nguyen",
            NickName = "nhan",
            Sex = Sex.Male,
            Status = UserStatus.Active,
            BirthDate = EntityTestData.CreateBirthDate(),
            AvatarUrl = null
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.FirstNameRequired);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowValidationException_WhenLastNameIsBlank(string lastName)
    {
        var action = () => new User
        {
            Id = Guid.NewGuid(),
            Email = "nhan@example.com",
            FirstName = "Nhan",
            LastName = lastName,
            NickName = "nhan",
            Sex = Sex.Male,
            Status = UserStatus.Active,
            BirthDate = EntityTestData.CreateBirthDate(),
            AvatarUrl = null
        };

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.LastNameRequired);
    }

    [Fact]
    public void SetNickName_ShouldAllowNull()
    {
        var user = CreateUser();

        user.NickName = null;

        user.NickName.Should().BeNull();
    }

    [Fact]
    public void SetNickName_ShouldThrowValidationException_WhenNickNameExceedsMaxLength()
    {
        var user = CreateUser();
        var nickName = new string('a', UserProfileValidationRules.NickNameMaxLength + 1);
        var action = () => user.NickName = nickName;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.NickNameMaxLengthExceeded);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenBirthDateIsInTheFuture()
    {
        var user = CreateUser();
        var action = () => user.BirthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.BirthDateCannotBeFuture);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenAgeIsBelowMinimum()
    {
        var user = CreateUser();
        var action = () => user.BirthDate = EntityTestData.CreateBirthDate(UserProfileValidationRules.MinAge - 1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.AgeCannotBeLessThan);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenAgeExceedsMaximum()
    {
        var user = CreateUser();
        var action = () => user.BirthDate = EntityTestData.CreateBirthDate(UserProfileValidationRules.MaxAge + 1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.AgeCannotExceed);
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = "nhan@example.com",
            FirstName = "  Nhan  ",
            LastName = "  Nguyen  ",
            NickName = "  nhan  ",
            Sex = Sex.Male,
            Status = UserStatus.Active,
            BirthDate = EntityTestData.CreateBirthDate(),
            AvatarUrl = "avatar.png"
        };
    }
}
