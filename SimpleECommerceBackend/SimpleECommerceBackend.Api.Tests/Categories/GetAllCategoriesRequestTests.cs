using FluentAssertions;
using SimpleECommerceBackend.Api.Dtos.Common.Filter;
using SimpleECommerceBackend.Api.Dtos.Common.Sorting;
using SimpleECommerceBackend.Api.Dtos.V1.Categories;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Utils;
using SimpleECommerceBackend.Infrastructure.Extensions;

namespace SimpleECommerceBackend.Api.Tests.Categories;

public class GetAllCategoriesRequestTests
{
    [Fact]
    public void ToQuery_ShouldBuildContentHash_ForBasicFilterAndSortRequest()
    {
        var request = CreateRequest();

        var query = GetAllCategoriesRequest.ToQuery(request);
        var hash = query.GetContentHash();

        hash.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ApplyFilteringAndSorting_ShouldHandleBasicFilterAndSortRequest()
    {
        var categories = CreateCategories();
        var request = CreateRequest();
        var query = GetAllCategoriesRequest.ToQuery(request);

        var result = categories
            .ApplyFiltering(query)
            .ApplySorting(query)
            .ToList();

        result.Should().ContainSingle();
        result[0].Name.Should().Be("string");
    }

    private static GetAllCategoriesRequest CreateRequest()
    {
        return new GetAllCategoriesRequest
        {
            CurrentPage = 1,
            ItemsPerPage = 5,
            GroupPattern = null,
            FilterCriteria =
            [
                new FilterCriterionRequest
                {
                    FieldName = "name",
                    Operator = "==",
                    Values = ["string"],
                    DateTimeFilterOptions = null
                }
            ],
            SortFields =
            [
                new SortFieldRequest
                {
                    FieldName = "name",
                    IsAscending = true
                }
            ]
        };
    }

    private static IQueryable<Category> CreateCategories()
    {
        var adminId = UuidUtils.CreateV7();

        return new[]
        {
            new Category
            {
                Name = "string",
                Description = "Match",
                Status = CategoryStatus.Active,
                AdminId = adminId
            },
            new Category
            {
                Name = "zeta",
                Description = "No match",
                Status = CategoryStatus.Active,
                AdminId = adminId
            }
        }.AsQueryable();
    }
}
