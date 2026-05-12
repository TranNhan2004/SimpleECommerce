using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class UserProfileTests
{
    [Fact]
    public void Create_ShouldCreateUserProfile_WhenInputIsValid()
    {
        var userProfile = CreateUserProfile();

        userProfile.Email.Should().Be("nhan@example.com");
        userProfile.FirstName.Should().Be("Nhan");
        userProfile.LastName.Should().Be("Nguyen");
        userProfile.NickName.Should().Be("nhan");
        userProfile.Sex.Should().Be(Sex.Male);
        userProfile.Status.Should().Be(UserStatus.Active);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowValidationException_WhenFirstNameIsBlank(string firstName)
    {
        var action = () => new UserProfile
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
        var action = () => new UserProfile
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
        var userProfile = CreateUserProfile();

        userProfile.NickName = null;

        userProfile.NickName.Should().BeNull();
    }

    [Fact]
    public void SetNickName_ShouldThrowValidationException_WhenNickNameExceedsMaxLength()
    {
        var userProfile = CreateUserProfile();
        var nickName = new string('a', UserProfileValidationRules.NickNameMaxLength + 1);
        var action = () => userProfile.NickName = nickName;

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.NickNameMaxLengthExceeded);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenBirthDateIsInTheFuture()
    {
        var userProfile = CreateUserProfile();
        var action = () => userProfile.BirthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.BirthDateCannotBeFuture);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenAgeIsBelowMinimum()
    {
        var userProfile = CreateUserProfile();
        var action = () => userProfile.BirthDate = EntityTestData.CreateBirthDate(UserProfileValidationRules.MinAge - 1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.AgeCannotBeLessThan);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenAgeExceedsMaximum()
    {
        var userProfile = CreateUserProfile();
        var action = () => userProfile.BirthDate = EntityTestData.CreateBirthDate(UserProfileValidationRules.MaxAge + 1);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCodes.AgeCannotExceed);
    }

    private static UserProfile CreateUserProfile()
    {
        return new UserProfile
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
