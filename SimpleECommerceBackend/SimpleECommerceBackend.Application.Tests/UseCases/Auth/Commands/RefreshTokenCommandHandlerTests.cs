using FluentAssertions;
using Moq;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Auth;
using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Application.UseCases.Auth.Commands;

namespace SimpleECommerceBackend.Application.Tests.UseCases.Auth.Commands;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IKeycloakTokenService> _keycloakTokenServiceMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _keycloakTokenServiceMock = new Mock<IKeycloakTokenService>();

        _handler = new RefreshTokenCommandHandler(
            _keycloakTokenServiceMock.Object
        );
    }

    // ---------- Happy path ----------

    [Fact]
    public async Task Handle_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        // Arrange
        var command = new RefreshTokenCommand(
            RefreshToken: "valid_refresh_token"
        );

        var tokenResponse = new KeycloakTokenResponse
        {
            AccessToken = "new_access_token",
            RefreshToken = "new_refresh_token",
            ExpiresIn = 3600,
            RefreshExpiresIn = 7200,
            TokenType = "Bearer",
            Scope = "profile email"
        };

        _keycloakTokenServiceMock
            .Setup(x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be(tokenResponse.AccessToken);
        result.RefreshToken.Should().Be(tokenResponse.RefreshToken);
        result.ExpiresIn.Should().Be(tokenResponse.ExpiresIn);

        _keycloakTokenServiceMock.Verify(
            x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnDifferentTokens_AfterRefresh()
    {
        // Arrange
        var command = new RefreshTokenCommand(
            RefreshToken: "old_refresh_token"
        );

        var tokenResponse = new KeycloakTokenResponse
        {
            AccessToken = "completely_new_access_token",
            RefreshToken = "completely_new_refresh_token",
            ExpiresIn = 3600,
            RefreshExpiresIn = 7200,
            TokenType = "Bearer",
            Scope = "profile email"
        };

        _keycloakTokenServiceMock
            .Setup(x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AccessToken.Should().NotBe(command.RefreshToken);
        result.RefreshToken.Should().NotBe(command.RefreshToken);
        result.AccessToken.Should().Be("completely_new_access_token");
        result.RefreshToken.Should().Be("completely_new_refresh_token");
    }

    [Fact]
    public async Task Handle_ShouldPreserveExpirationValues_FromKeycloakResponse()
    {
        // Arrange
        var command = new RefreshTokenCommand(
            RefreshToken: "valid_refresh_token"
        );

        var tokenResponse = new KeycloakTokenResponse
        {
            AccessToken = "new_access_token",
            RefreshToken = "new_refresh_token",
            ExpiresIn = 1800, // 30 minutes
            RefreshExpiresIn = 3600, // 60 minutes
            TokenType = "Bearer",
            Scope = "profile email"
        };

        _keycloakTokenServiceMock
            .Setup(x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ExpiresIn.Should().Be(1800);
    }

    // ---------- Error scenarios ----------

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var command = new RefreshTokenCommand(
            RefreshToken: "invalid_refresh_token"
        );

        _keycloakTokenServiceMock
            .Setup(x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid refresh token"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid refresh token");

        _keycloakTokenServiceMock.Verify(
            x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenRefreshTokenIsExpired()
    {
        // Arrange
        var command = new RefreshTokenCommand(
            RefreshToken: "expired_refresh_token"
        );

        _keycloakTokenServiceMock
            .Setup(x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Refresh token expired"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Refresh token expired");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_ShouldThrow_WhenRefreshTokenIsEmpty(string? emptyToken)
    {
        // Arrange
        var command = new RefreshTokenCommand(
            RefreshToken: emptyToken!
        );

        _keycloakTokenServiceMock
            .Setup(x => x.RefreshTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Refresh token cannot be empty"));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
