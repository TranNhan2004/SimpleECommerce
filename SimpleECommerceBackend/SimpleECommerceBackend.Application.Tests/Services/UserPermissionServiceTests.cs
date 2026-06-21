using FluentAssertions;
using Moq;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Services.Uam;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Tests.Services;

public class UserPermissionServiceTests
{
    [Fact]
    public async Task GetPermissionCodesAsync_ShouldReturnCachedPermissions_WhenCacheHit()
    {
        var cacheServiceMock = new Mock<ICacheService>();
        var permissionRepositoryMock = new Mock<IPermissionRepository>();
        var userServiceMock = new Mock<IUserService>();
        var userId = Guid.NewGuid();
        var cachedPermissions = new List<string> { "categories.read", "users.self.read" };

        userServiceMock
            .Setup(service => service.IsActiveUserAsync(userId))
            .ReturnsAsync(true);

        cacheServiceMock
            .Setup(service => service.GetAsync<List<string>>(PermissionCacheKeys.GetPermissionSetKey(userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedPermissions);

        var sut = new PermissionService(
            cacheServiceMock.Object,
            permissionRepositoryMock.Object
        );

        var result = await sut.GetPermissionCodesByUserIdAsync(userId);

        result.Should().Equal(cachedPermissions);
        permissionRepositoryMock.Verify(
            repository => repository.FindCodesByUserIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetPermissionCodesAsync_ShouldThrowUnauthorizedException_WhenUserIsInactive()
    {
        var cacheServiceMock = new Mock<ICacheService>();
        var permissionRepositoryMock = new Mock<IPermissionRepository>();
        var userServiceMock = new Mock<IUserService>();
        var userId = Guid.NewGuid();

        userServiceMock
            .Setup(service => service.IsActiveUserAsync(userId))
            .ReturnsAsync(false);

        var sut = new PermissionService(
            cacheServiceMock.Object,
            permissionRepositoryMock.Object
        );

        var action = async () => await sut.GetPermissionCodesByUserIdAsync(userId);

        (await action.Should().ThrowAsync<UnauthorizedException>())
            .Which.ErrorCode.Should().Be(UserErrorCodes.InactiveUser);
    }
}
