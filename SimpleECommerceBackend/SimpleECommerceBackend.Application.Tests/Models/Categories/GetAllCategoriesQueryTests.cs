using FluentAssertions;
using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Application.Tests.Models.Categories;

public class GetAllCategoriesQueryTests
{
    [Fact]
    public void GetContentHash_ShouldReturnSameHash_ForEquivalentQueries()
    {
        var firstQuery = new GetAllCategoriesQuery
        {
            CurrentPage = 2,
            ItemsPerPage = 25
        };

        var secondQuery = new GetAllCategoriesQuery
        {
            CurrentPage = 2,
            ItemsPerPage = 25
        };

        firstQuery.GetContentHash().Should().Be(secondQuery.GetContentHash());
    }

    [Fact]
    public void GetContentHash_ShouldReturnDifferentHash_WhenQueryChanges()
    {
        var firstQuery = new GetAllCategoriesQuery
        {
            CurrentPage = 1,
            ItemsPerPage = 25
        };

        var secondQuery = new GetAllCategoriesQuery
        {
            CurrentPage = 2,
            ItemsPerPage = 25
        };

        firstQuery.GetContentHash().Should().NotBe(secondQuery.GetContentHash());
    }
}