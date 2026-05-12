using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Translation;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class TranslationEntryTests
{
    [Fact]
    public void Constructor_ShouldCreateTranslationEntry_WhenInputIsValid()
    {
        var translationEntry = new TranslationEntry(
            SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            "  Product  ",
            "  Name  ",
            SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            "  EN-US  ",
            "  Book  ");

        translationEntry.EntityName.Should().Be("Product");
        translationEntry.FieldName.Should().Be("Name");
        translationEntry.Locale.Should().Be("en-us");
        translationEntry.Value.Should().Be("Book");
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenRowIdIsEmpty()
    {
        var action = () => new TranslationEntry(
            SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            "Product",
            "Name",
            Guid.Empty,
            "en-us",
            "Book");

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(TranslationEntryErrorCodes.RowIdRequired);
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenLocaleIsBlank()
    {
        var action = () => new TranslationEntry(
            SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            "Product",
            "Name",
            SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            " ",
            "Book");

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(TranslationEntryErrorCodes.LocaleRequired);
    }

    [Fact]
    public void UpdateValue_ShouldTrimAndReplaceValue()
    {
        var translationEntry = new TranslationEntry(
            SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            "Product",
            "Name",
            SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7(),
            "en-us",
            "Book");

        translationEntry.UpdateValue("  Updated Book  ");

        translationEntry.Value.Should().Be("Updated Book");
    }
}
