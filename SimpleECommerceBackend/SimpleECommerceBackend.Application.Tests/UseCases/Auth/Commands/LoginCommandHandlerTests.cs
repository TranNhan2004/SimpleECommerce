using FluentAssertions;
using Moq;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Application.UseCases.Auth.Commands;
using SimpleECommerceBackend.Application.Models.Auth;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Tests.UseCases.Auth.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IKeycloakTokenService> _keycloakTokenServiceMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _keycloakTokenServiceMock = new Mock<IKeycloakTokenService>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new LoginCommandHandler(
            _keycloakTokenServiceMock.Object,
            _userProfileRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    // ---------- Happy path ----------

    [Fact]
    public async Task Handle_ShouldReturnLoginResult_WhenUserProfileExists()
    {
        // Arrange
        var command = new LoginCommand(
            Email: "test@example.com",
            Password: "Test@123"
        );

        var keycloakUserId = Guid.NewGuid();
        var tokenResponse = new KeycloakTokenResponse
        {
            AccessToken = "mock_access_token",
            RefreshToken = "mock_refresh_token",
            ExpiresIn = 3600,
            RefreshExpiresIn = 7200,
            TokenType = "Bearer",
            Scope = "profile email"
        };

        var userInfoResponse = new KeycloakUserInfoResponse
        {
            Sub = keycloakUserId.ToString(),
            Email = command.Email,
            EmailVerified = true,
            PreferredUsername = command.Email,
            GivenName = "John",
            FamilyName = "Doe",
            Roles = new List<string> { "customer" }
        };

        var existingUserProfile = UserProfile.Create(
            keycloakUserId,
            command.Email,
            "John",
            "Doe",
            null,
            Sex.Male,
            new DateOnly(1990, 1, 1),
            null
        );

        _keycloakTokenServiceMock
            .Setup(x => x.GetTokenAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _keycloakTokenServiceMock
            .Setup(x => x.GetUserInfoAsync(tokenResponse.AccessToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfoResponse);

        _userProfileRepositoryMock
            .Setup(x => x.FindByIdAsync(keycloakUserId))
            .ReturnsAsync(existingUserProfile);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(keycloakUserId);
        result.Email.Should().Be(command.Email);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Role.Should().Be(Role.Customer);
        result.AccessToken.Should().Be(tokenResponse.AccessToken);
        result.RefreshToken.Should().Be(tokenResponse.RefreshToken);
        result.ExpiresIn.Should().Be(tokenResponse.ExpiresIn);

        _keycloakTokenServiceMock.Verify(
            x => x.GetTokenAsync(command.Email, command.Password, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _keycloakTokenServiceMock.Verify(
            x => x.GetUserInfoAsync(tokenResponse.AccessToken, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _userProfileRepositoryMock.Verify(
            x => x.FindByIdAsync(keycloakUserId),
            Times.Once
        );

        // Should not save since user profile already exists
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateUserProfile_WhenUserProfileDoesNotExist()
    {
        // Arrange
        var command = new LoginCommand(
            Email: "newuser@example.com",
            Password: "Test@123"
        );

        var keycloakUserId = Guid.NewGuid();
        var tokenResponse = new KeycloakTokenResponse
        {
            AccessToken = "mock_access_token",
            RefreshToken = "mock_refresh_token",
            ExpiresIn = 3600,
            RefreshExpiresIn = 7200,
            TokenType = "Bearer",
            Scope = "profile email"
        };

        var userInfoResponse = new KeycloakUserInfoResponse
        {
            Sub = keycloakUserId.ToString(),
            Email = command.Email,
            EmailVerified = true,
            PreferredUsername = command.Email,
            GivenName = "Jane",
            FamilyName = "Smith",
            Roles = new List<string> { "seller" }
        };

        _keycloakTokenServiceMock
            .Setup(x => x.GetTokenAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _keycloakTokenServiceMock
            .Setup(x => x.GetUserInfoAsync(tokenResponse.AccessToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfoResponse);

        _userProfileRepositoryMock
            .Setup(x => x.FindByIdAsync(keycloakUserId))
            .ReturnsAsync((UserProfile?)null);

        _userProfileRepositoryMock
            .Setup(x => x.Add(It.IsAny<UserProfile>()))
            .Returns((UserProfile up) => up);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(keycloakUserId);
        result.Email.Should().Be(command.Email);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Role.Should().Be(Role.Seller);
        result.AccessToken.Should().Be(tokenResponse.AccessToken);

        _userProfileRepositoryMock.Verify(
            x => x.Add(It.Is<UserProfile>(up =>
                up.Id == keycloakUserId &&
                up.Email == command.Email &&
                up.FirstName == "Jane" &&
                up.LastName == "Smith"
            )),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultRole_WhenNoRolesInToken()
    {
        // Arrange
        var command = new LoginCommand(
            Email: "test@example.com",
            Password: "Test@123"
        );

        var keycloakUserId = Guid.NewGuid();
        var tokenResponse = new KeycloakTokenResponse
        {
            AccessToken = "mock_access_token",
            RefreshToken = "mock_refresh_token",
            ExpiresIn = 3600,
            RefreshExpiresIn = 7200,
            TokenType = "Bearer",
            Scope = "profile email"
        };

        var userInfoResponse = new KeycloakUserInfoResponse
        {
            Sub = keycloakUserId.ToString(),
            Email = command.Email,
            EmailVerified = true,
            PreferredUsername = command.Email,
            GivenName = "John",
            FamilyName = "Doe",
            Roles = [] // Empty roles
        };

        var existingUserProfile = UserProfile.Create(
            keycloakUserId,
            command.Email,
            "John",
            "Doe",
            null,
            Sex.Male,
            new DateOnly(1990, 1, 1),
            null
        );

        _keycloakTokenServiceMock
            .Setup(x => x.GetTokenAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _keycloakTokenServiceMock
            .Setup(x => x.GetUserInfoAsync(tokenResponse.AccessToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfoResponse);

        _userProfileRepositoryMock
            .Setup(x => x.FindByIdAsync(keycloakUserId))
            .ReturnsAsync(existingUserProfile);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be(Role.Customer);
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultNames_WhenNamesAreEmpty()
    {
        // Arrange
        var command = new LoginCommand(
            Email: "test@example.com",
            Password: "Test@123"
        );

        var keycloakUserId = Guid.NewGuid();
        var tokenResponse = new KeycloakTokenResponse
        {
            AccessToken = "mock_access_token",
            RefreshToken = "mock_refresh_token",
            ExpiresIn = 3600,
            RefreshExpiresIn = 7200,
            TokenType = "Bearer",
            Scope = "profile email"
        };

        var userInfoResponse = new KeycloakUserInfoResponse
        {
            Sub = keycloakUserId.ToString(),
            Email = command.Email,
            EmailVerified = true,
            PreferredUsername = command.Email,
            GivenName = string.Empty, // Empty given name
            FamilyName = string.Empty, // Empty family name
            Roles = new List<string> { "customer" }
        };

        _keycloakTokenServiceMock
            .Setup(x => x.GetTokenAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        _keycloakTokenServiceMock
            .Setup(x => x.GetUserInfoAsync(tokenResponse.AccessToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userInfoResponse);

        _userProfileRepositoryMock
            .Setup(x => x.FindByIdAsync(keycloakUserId))
            .ReturnsAsync((UserProfile?)null);

        _userProfileRepositoryMock
            .Setup(x => x.Add(It.IsAny<UserProfile>()))
            .Returns((UserProfile up) => up);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        _userProfileRepositoryMock.Verify(
            x => x.Add(It.Is<UserProfile>(up =>
                up.FirstName == "User" &&
                up.LastName == "Name"
            )),
            Times.Once
        );
    }

    // ---------- Error scenarios ----------

    [Fact]
    public async Task Handle_ShouldThrow_WhenKeycloakTokenServiceThrows()
    {
        // Arrange
        var command = new LoginCommand(
            Email: "test@example.com",
            Password: "Test@123"
        );

        _keycloakTokenServiceMock
            .Setup(x => x.GetTokenAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedException("Invalid credentials"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid credentials");

        _keycloakTokenServiceMock.Verify(
            x => x.GetTokenAsync(command.Email, command.Password, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
