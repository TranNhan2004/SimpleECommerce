using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Moq;
using SimpleECommerceBackend.Infrastructure.Options.Translation;
using SimpleECommerceBackend.Infrastructure.Services.Translation;

namespace SimpleECommerceBackend.Infrastructure.Tests.Services.Translation;

public class JsonStaticTextLocalizerTests
{
    [Fact]
    public void LocalizeError_ShouldResolveTemplateFromNestedErrorSections()
    {
        const string json = """
                            {
                              "fields": {
                                "Product": {
                                  "Name": "Product name"
                                },
                                "Category": {
                                  "Name": "Category name"
                                }
                              },
                              "errors": {
                                "Product": {
                                  "NameRequired": "{field} is required."
                                }
                              }
                            }
                            """;

        var contentRootPath = CreateContentRoot(json);

        try
        {
            var sut = CreateSut(contentRootPath);

            var result = sut.LocalizeError(
                "Product_NameRequired",
                new Dictionary<string, object?> { ["field"] = "Name" },
                "en"
            );

            result.Message.Should().Be("Product name is required.");
            result.FieldKey.Should().Be("Name");
            result.FieldDisplayName.Should().Be("Product name");
        }
        finally
        {
            Directory.Delete(contentRootPath, true);
        }
    }

    [Fact]
    public void LocalizeError_ShouldKeepSupportingFlatErrorKeys()
    {
        const string json = """
                            {
                              "fields": {
                                "Name": "Name"
                              },
                              "errors": {
                                "Product_NameRequired": "{field} is required."
                              }
                            }
                            """;

        var contentRootPath = CreateContentRoot(json);

        try
        {
            var sut = CreateSut(contentRootPath);

            var result = sut.LocalizeError(
                "Product_NameRequired",
                new Dictionary<string, object?> { ["field"] = "Name" },
                "en"
            );

            result.Message.Should().Be("Name is required.");
        }
        finally
        {
            Directory.Delete(contentRootPath, true);
        }
    }

    [Fact]
    public void LocalizeError_ShouldKeepSupportingFlatFieldKeys()
    {
        const string json = """
                            {
                              "fields": {
                                "Name": "Name"
                              },
                              "errors": {
                                "Product": {
                                  "NameRequired": "{field} is required."
                                }
                              }
                            }
                            """;

        var contentRootPath = CreateContentRoot(json);

        try
        {
            var sut = CreateSut(contentRootPath);

            var result = sut.LocalizeError(
                "Product_NameRequired",
                new Dictionary<string, object?> { ["field"] = "Name" },
                "en"
            );

            result.Message.Should().Be("Name is required.");
        }
        finally
        {
            Directory.Delete(contentRootPath, true);
        }
    }

    private static JsonStaticTextLocalizer CreateSut(string contentRootPath)
    {
        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.SetupGet(environment => environment.ContentRootPath).Returns(contentRootPath);

        var options = Microsoft.Extensions.Options.Options.Create(new TranslationOptions
        {
            DefaultLocale = "en",
            StaticResourcesPath = "ErrorMessages"
        });

        return new JsonStaticTextLocalizer(hostEnvironment.Object, options);
    }

    private static string CreateContentRoot(string json)
    {
        var contentRootPath = Path.Combine(Path.GetTempPath(), $"json-localizer-tests-{SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7():N}");
        var errorMessagesPath = Path.Combine(contentRootPath, "ErrorMessages");

        Directory.CreateDirectory(errorMessagesPath);
        File.WriteAllText(Path.Combine(errorMessagesPath, "errormessages.en.json"), json);

        return contentRootPath;
    }
}