using FluentAssertions;
using SimpleECommerceBackend.Application.Enums;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Infrastructure.Extensions;

namespace SimpleECommerceBackend.Infrastructure.Tests.Extensions;

public class FilterExtensionTests
{
    [Fact]
    public void ApplyFiltering_ShouldThrowValidationException_WhenCriterionValueCannotBeParsed()
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
                    FieldName = "createdAt",
                    Operator = FilterOperator.Equal,
                    Values = ["not-a-date"]
                }
            ]
        };

        var action = () => categories.ApplyFiltering(query).ToList();

        action.Should()
            .Throw<ValidationException>()
            .Which.ErrorCode.Should()
            .Be(FilterErrorCodes.InvalidValue);
    }

    [Fact]
    public void ApplyFiltering_ShouldFilterSuccessfully_WhenCriterionValueMatchesMappedFieldType()
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
                    Values = ["2"]
                }
            ]
        };

        var result = categories.ApplyFiltering(query).ToList();

        result.Should().ContainSingle(category => category.Name == "2");
    }

    private static IQueryable<Category> CreateCategories()
    {
        var adminId = SimpleECommerceBackend.Domain.Utils.UuidUtils.CreateV7();

        return new[]
        {
            new Category
            {
                Name = "2",
                Description = "Matches the filter",
                AdminId = adminId
            },
            new Category
            {
                Name = "Other",
                Description = "Does not match",
                AdminId = adminId
            }
        }.AsQueryable();
    }
}
