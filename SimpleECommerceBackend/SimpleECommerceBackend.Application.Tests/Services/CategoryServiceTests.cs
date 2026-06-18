using FluentAssertions;
using Moq;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Application.Services.Business;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Tests.Services;

public class CategoryServiceTests
{
    [Fact]
    public async Task GetAllCategoriesAsync_ShouldFallBackToRepository_WhenCacheReadHangs()
    {
        var cacheServiceMock = new Mock<ICacheService>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var query = new GetAllCategoriesQuery
        {
            CurrentPage = 1,
            ItemsPerPage = 5
        };
        var repositoryResult = CreateFilterResult();

        cacheServiceMock
            .Setup(service => service.GetAsync<GetAllCategoriesResult>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Delay(Timeout.InfiniteTimeSpan).ContinueWith(_ => (GetAllCategoriesResult?)null));

        cacheServiceMock
            .Setup(service => service.SetAsync(It.IsAny<string>(), It.IsAny<GetAllCategoriesResult>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        categoryRepositoryMock
            .Setup(repository => repository.FindAllWithFilterAsync(query))
            .ReturnsAsync(repositoryResult);

        var sut = new CategoryService(cacheServiceMock.Object, categoryRepositoryMock.Object);

        var result = await sut.GetAllCategoriesAsync(query).WaitAsync(TimeSpan.FromSeconds(2));

        result.Items.Should().HaveCount(1);
        categoryRepositoryMock.Verify(repository => repository.FindAllWithFilterAsync(query), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnRepositoryResult_WhenCacheWriteHangs()
    {
        var cacheServiceMock = new Mock<ICacheService>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var query = new GetAllCategoriesQuery
        {
            CurrentPage = 1,
            ItemsPerPage = 5
        };
        var repositoryResult = CreateFilterResult();

        cacheServiceMock
            .Setup(service => service.GetAsync<GetAllCategoriesResult>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetAllCategoriesResult?)null);

        cacheServiceMock
            .Setup(service => service.SetAsync(It.IsAny<string>(), It.IsAny<GetAllCategoriesResult>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.Delay(Timeout.InfiniteTimeSpan));

        categoryRepositoryMock
            .Setup(repository => repository.FindAllWithFilterAsync(query))
            .ReturnsAsync(repositoryResult);

        var sut = new CategoryService(cacheServiceMock.Object, categoryRepositoryMock.Object);

        var result = await sut.GetAllCategoriesAsync(query).WaitAsync(TimeSpan.FromSeconds(2));

        result.Items.Should().HaveCount(1);
        categoryRepositoryMock.Verify(repository => repository.FindAllWithFilterAsync(query), Times.Once);
    }

    private static FilterResult<CategoryItem> CreateFilterResult()
    {
        return new FilterResult<CategoryItem>
        {
            Items =
            [
                new CategoryItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Category",
                    Description = "Description"
                }
            ],
            CurrentPage = 1,
            ItemsPerPage = 5,
            TotalItems = 1,
            TotalPages = 1
        };
    }
}
