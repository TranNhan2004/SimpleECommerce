using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Moq;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Services.Keycloak;
using SimpleECommerceBackend.Application.Models.Auth.ConfirmEmail;
using SimpleECommerceBackend.Application.UseCases.Auth.ConfirmEmail;
using SimpleECommerceBackend.Domain.Entities;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Tests.UseCases.Auth;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<IEmailVerificationRepository> _emailVerificationRepositoryMock;
    private readonly Mock<IKeycloakAdminService> _keycloakAdminServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ConfirmEmailCommandHandler _handler;

    public ConfirmEmailCommandHandlerTests()
    {
        _emailVerificationRepositoryMock = new Mock<IEmailVerificationRepository>();
        _keycloakAdminServiceMock = new Mock<IKeycloakAdminService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new ConfirmEmailCommandHandler(
            _emailVerificationRepositoryMock.Object,
            _keycloakAdminServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldConfirmEmail_WhenTokenIsValid()
    {
        var token = "valid-token";
        var userId = Guid.NewGuid();
        var verification = EmailVerification.Create(
            userId,
            "test@example.com",
            HashToken(token),
            DateTimeOffset.UtcNow.AddHours(1)
        );

        _emailVerificationRepositoryMock
            .Setup(x => x.FindByTokenHashAsync(HashToken(token)))
            .ReturnsAsync(verification);

        _keycloakAdminServiceMock
            .Setup(x => x.MarkEmailAsVerifiedAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _emailVerificationRepositoryMock
            .Setup(x => x.Update(verification))
            .Returns(verification);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new ConfirmEmailCommand(token), CancellationToken.None);

        result.Email.Should().Be("test@example.com");
        result.AlreadyConfirmed.Should().BeFalse();
        result.Message.Should().Be("Email verified successfully");
        result.ConfirmedAt.Should().BeAfter(DateTimeOffset.UtcNow.AddMinutes(-1));

        _keycloakAdminServiceMock.Verify(
            x => x.MarkEmailAsVerifiedAsync(userId.ToString(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _emailVerificationRepositoryMock.Verify(x => x.Update(verification), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyConfirmed_WhenTokenWasAlreadyUsed()
    {
        var token = "already-confirmed-token";
        var verification = EmailVerification.Create(
            Guid.NewGuid(),
            "test@example.com",
            HashToken(token),
            DateTimeOffset.UtcNow.AddHours(1)
        );
        verification.Confirm(DateTimeOffset.UtcNow);

        _emailVerificationRepositoryMock
            .Setup(x => x.FindByTokenHashAsync(HashToken(token)))
            .ReturnsAsync(verification);

        var result = await _handler.Handle(new ConfirmEmailCommand(token), CancellationToken.None);

        result.Email.Should().Be("test@example.com");
        result.AlreadyConfirmed.Should().BeTrue();
        result.Message.Should().Be("Email is already verified");

        _keycloakAdminServiceMock.Verify(
            x => x.MarkEmailAsVerifiedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenTokenDoesNotExist()
    {
        var token = "missing-token";

        _emailVerificationRepositoryMock
            .Setup(x => x.FindByTokenHashAsync(HashToken(token)))
            .ReturnsAsync((EmailVerification?)null);

        var act = async () => await _handler.Handle(new ConfirmEmailCommand(token), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Email verification token is invalid or does not exist");

        _keycloakAdminServiceMock.Verify(
            x => x.MarkEmailAsVerifiedAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}