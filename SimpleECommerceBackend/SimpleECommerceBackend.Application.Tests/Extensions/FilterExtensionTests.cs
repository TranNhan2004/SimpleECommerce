using FluentAssertions;
using SimpleECommerceBackend.Application.Enums;
using SimpleECommerceBackend.Application.Extensions;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Tests.Extensions;

public class FilterExtensionTests
{
    [Fact]
    public void ToFilterResult_ShouldThrowValidationException_WhenCriterionFieldTypeDoesNotMatchMappedField()
    {
        var categories = CreateCategories();
        var query = new GetAllCategoriesQueryForAdmin
        {
            CurrentPage = 1,
            ItemsPerPage = 10,
            FilterCriteria =
            [
                new FilterCriterion
                {
                    FieldName = "name",
                    Operator = FilterOperator.Equal,
                    FieldType = FieldType.Int,
                    Values = ["2"]
                }
            ]
        };

        var action = () => categories.ToFilterResult(query);

        action.Should()
            .Throw<ValidationException>()
            .Which.ErrorCode.Should()
            .Be(FilterErrorCodes.InvalidValue);
    }

    [Fact]
    public void ToFilterResult_ShouldFilterSuccessfully_WhenCriterionFieldTypeMatchesMappedField()
    {
        var categories = CreateCategories();
        var query = new GetAllCategoriesQueryForAdmin
        {
            CurrentPage = 1,
            ItemsPerPage = 10,
            FilterCriteria =
            [
                new FilterCriterion
                {
                    FieldName = "name",
                    Operator = FilterOperator.Equal,
                    FieldType = FieldType.String,
                    Values = ["2"]
                }
            ]
        };

        var result = categories.ToFilterResult(query);

        result.TotalItems.Should().Be(1);
        result.Items.Should().ContainSingle(category => category.Name == "2");
    }

    private static IQueryable<Category> CreateCategories()
    {
        var adminId = Guid.NewGuid();

        return new[]
        {
            Category.Create("2", "Matches the filter", adminId),
            Category.Create("Other", "Does not match", adminId)
        }.AsQueryable();
    }
}
