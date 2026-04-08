using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Tests.Utils;

public class SexUtilsTests
{
    [Theory]
    [InlineData("male", Sex.Male)]
    [InlineData("female", Sex.Female)]
    [InlineData("other", Sex.Other)]
    [InlineData(" Male ", Sex.Male)]
    public void Parse_ShouldReturnSex_WhenInputIsValid(string input, Sex expected)
    {
        var result = SexUtils.Parse(input);

        result.Should().Be(expected);
    }

    [Fact]
    public void Parse_ShouldThrowValidationException_WhenInputIsInvalid()
    {
        var action = () => SexUtils.Parse("unknown");

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(SexErrorCode.InvalidSex);
    }

    [Fact]
    public void ToName_ShouldReturnLowercaseName_WhenSexIsSupported()
    {
        SexUtils.ToName(Sex.Female).Should().Be("female");
    }
}
