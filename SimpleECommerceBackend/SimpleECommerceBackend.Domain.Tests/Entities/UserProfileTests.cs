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
        var action = () => UserProfile.Create(
            Guid.NewGuid(),
            "nhan@example.com",
            firstName,
            "Nguyen",
            "nhan",
            Sex.Male,
            EntityTestData.CreateBirthDate(),
            null);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCode.FirstNameRequired);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowValidationException_WhenLastNameIsBlank(string lastName)
    {
        var action = () => UserProfile.Create(
            Guid.NewGuid(),
            "nhan@example.com",
            "Nhan",
            lastName,
            "nhan",
            Sex.Male,
            EntityTestData.CreateBirthDate(),
            null);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCode.LastNameRequired);
    }

    [Fact]
    public void SetNickName_ShouldAllowNull()
    {
        var userProfile = CreateUserProfile();

        userProfile.SetNickName(null);

        userProfile.NickName.Should().BeNull();
    }

    [Fact]
    public void SetNickName_ShouldThrowValidationException_WhenNickNameExceedsMaxLength()
    {
        var userProfile = CreateUserProfile();
        var nickName = new string('a', UserProfileConstants.NickNameMaxLength + 1);
        var action = () => userProfile.SetNickName(nickName);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCode.NickNameMaxLengthExceeded);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenBirthDateIsInTheFuture()
    {
        var userProfile = CreateUserProfile();
        var action = () => userProfile.SetBirthDate(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1));

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCode.BirthDateCannotBeFuture);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenAgeIsBelowMinimum()
    {
        var userProfile = CreateUserProfile();
        var action = () => userProfile.SetBirthDate(EntityTestData.CreateBirthDate(UserProfileConstants.MinAge - 1));

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCode.AgeCannotBeLessThan);
    }

    [Fact]
    public void SetBirthDate_ShouldThrowValidationException_WhenAgeExceedsMaximum()
    {
        var userProfile = CreateUserProfile();
        var action = () => userProfile.SetBirthDate(EntityTestData.CreateBirthDate(UserProfileConstants.MaxAge + 1));

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(UserProfileErrorCode.AgeCannotExceed);
    }

    private static UserProfile CreateUserProfile()
    {
        return UserProfile.Create(
            Guid.NewGuid(),
            "nhan@example.com",
            "  Nhan  ",
            "  Nguyen  ",
            "  nhan  ",
            Sex.Male,
            EntityTestData.CreateBirthDate(),
            "avatar.png");
    }
}
