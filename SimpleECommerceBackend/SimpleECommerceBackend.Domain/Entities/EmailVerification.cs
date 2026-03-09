using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Entities;

public class EmailVerification : Entity, ICreatedTrackable
{
    private EmailVerification()
    {
    }

    private EmailVerification(Guid userId, string email, string tokenHash, DateTimeOffset expiresAt)
    {
        SetId(Guid.NewGuid());
        SetUserId(userId);
        SetEmail(email);
        SetTokenHash(tokenHash);
        SetExpiresAt(expiresAt);
    }

    public Guid UserId { get; private set; }
    public UserProfile? User { get; private set; }
    public string Email { get; private set; } = null!;
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsConfirmed => ConfirmedAt.HasValue;

    public static EmailVerification Create(
        Guid userId,
        string email,
        string tokenHash,
        DateTimeOffset expiresAt
    )
    {
        return new EmailVerification(userId, email, tokenHash, expiresAt);
    }

    public bool IsExpired(DateTimeOffset now)
    {
        return now >= ExpiresAt;
    }

    public void Confirm(DateTimeOffset confirmedAt)
    {
        if (IsConfirmed)
            throw new BusinessException("Email is already verified");

        if (IsExpired(confirmedAt))
            throw new BusinessException("Email verification token has expired");

        ConfirmedAt = confirmedAt;
    }

    private void SetUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new BusinessException("User ID is required");

        UserId = userId;
    }

    private void SetEmail(string email)
    {
        Email = CredentialUtils.ValidateEmail(email);
    }

    private void SetTokenHash(string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new BusinessException("Verification token hash is required");

        var normalizedTokenHash = tokenHash.Trim();
        if (normalizedTokenHash.Length != EmailVerificationConstants.TokenHashLength)
            throw new BusinessException(
                $"Verification token hash must be {EmailVerificationConstants.TokenHashLength} characters"
            );

        TokenHash = normalizedTokenHash;
    }

    private void SetExpiresAt(DateTimeOffset expiresAt)
    {
        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new BusinessException("Verification expiration must be in the future");

        ExpiresAt = expiresAt;
    }
}