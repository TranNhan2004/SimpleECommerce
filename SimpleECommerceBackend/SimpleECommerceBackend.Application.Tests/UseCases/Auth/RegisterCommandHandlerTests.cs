using FluentAssertions;
using Moq;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Auth.Register;
using SimpleECommerceBackend.Application.Models.Keycloak;
using SimpleECommerceBackend.Application.UseCases.Auth.Register;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Tests.UseCases.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IKeycloakAdminService> _keycloakAdminServiceMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _keycloakAdminServiceMock = new Mock<IKeycloakAdminService>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RegisterCommandHandler(
            _keycloakAdminServiceMock.Object,
            _userProfileRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    // ---------- Happy path ----------

    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenInputIsValid()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "Test@123",
            FirstName: "John",
            LastName: "Doe",
            Role: "customer"
        );

        var keycloakUserId = Guid.NewGuid();
        var keycloakUserResponse = new CreateKeycloakUserResponse
        {
            KeycloakUserId = keycloakUserId.ToString(),
            Email = command.Email
        };

        _keycloakAdminServiceMock
            .Setup(x => x.UserExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _keycloakAdminServiceMock
            .Setup(x => x.CreateUserAsync(It.IsAny<CreateKeycloakUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(keycloakUserResponse);

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
        result.Email.Should().Be(command.Email);

        _keycloakAdminServiceMock.Verify(
            x => x.UserExistsAsync(command.Email, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _keycloakAdminServiceMock.Verify(
            x => x.CreateUserAsync(
                It.Is<CreateKeycloakUserRequest>(r =>
                    r.Email == command.Email &&
                    r.Password == command.Password &&
                    r.FirstName == command.FirstName &&
                    r.LastName == command.LastName &&
                    r.Role == "customer"
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        _userProfileRepositoryMock.Verify(
            x => x.Add(It.Is<UserProfile>(up =>
                up.Id == keycloakUserId &&
                up.Email == command.Email &&
                up.FirstName == command.FirstName &&
                up.LastName == command.LastName
            )),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Theory]
    [InlineData("customer")]
    [InlineData("seller")]
    [InlineData("admin")]
    public async Task Handle_ShouldAcceptValidRoles(string role)
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "Test@123",
            FirstName: "John",
            LastName: "Doe",
            Role: role
        );

        var keycloakUserResponse = new CreateKeycloakUserResponse
        {
            KeycloakUserId = Guid.NewGuid().ToString(),
            Email = command.Email
        };

        _keycloakAdminServiceMock
            .Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _keycloakAdminServiceMock
            .Setup(x => x.CreateUserAsync(It.IsAny<CreateKeycloakUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(keycloakUserResponse);

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
        result.Email.Should().Be(command.Email);

        _keycloakAdminServiceMock.Verify(
            x => x.CreateUserAsync(
                It.Is<CreateKeycloakUserRequest>(r => r.Role == role.ToLower()),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    // ---------- Error scenarios ----------

    [Fact]
    public async Task Handle_ShouldThrowBusinessException_WhenUserAlreadyExists()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "existing@example.com",
            Password: "Test@123",
            FirstName: "John",
            LastName: "Doe",
            Role: "customer"
        );

        _keycloakAdminServiceMock
            .Setup(x => x.UserExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("User with this email already exists");

        _keycloakAdminServiceMock.Verify(
            x => x.UserExistsAsync(command.Email, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _keycloakAdminServiceMock.Verify(
            x => x.CreateUserAsync(It.IsAny<CreateKeycloakUserRequest>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("user")]
    [InlineData("moderator")]
    [InlineData("")]
    public async Task Handle_ShouldThrowBusinessException_WhenRoleIsInvalid(string invalidRole)
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "Test@123",
            FirstName: "John",
            LastName: "Doe",
            Role: invalidRole
        );

        _keycloakAdminServiceMock
            .Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Invalid role. Must be one of: customer, seller, admin");

        _keycloakAdminServiceMock.Verify(
            x => x.CreateUserAsync(It.IsAny<CreateKeycloakUserRequest>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_ShouldNormalizeRoleToLowercase()
    {
        // Arrange
        var command = new RegisterCommand(
            Email: "test@example.com",
            Password: "Test@123",
            FirstName: "John",
            LastName: "Doe",
            Role: "CUSTOMER"
        );

        var keycloakUserResponse = new CreateKeycloakUserResponse
        {
            KeycloakUserId = Guid.NewGuid().ToString(),
            Email = command.Email
        };

        _keycloakAdminServiceMock
            .Setup(x => x.UserExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _keycloakAdminServiceMock
            .Setup(x => x.CreateUserAsync(It.IsAny<CreateKeycloakUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(keycloakUserResponse);

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

        _keycloakAdminServiceMock.Verify(
            x => x.CreateUserAsync(
                It.Is<CreateKeycloakUserRequest>(r => r.Role == "customer"),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
