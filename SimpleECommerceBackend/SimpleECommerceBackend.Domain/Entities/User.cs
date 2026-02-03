using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces;

namespace SimpleECommerceBackend.Domain.Entities;

public class User : EntityBase, IAuditable, ISoftDeletable
{
    private User()
    {
    }

    private User(
        string credentialId,
        string email,
        string phoneNumber,
        string firstName,
        string lastName,
        DateOnly birthDate
    )
    {
        SetCredentialId(credentialId);
        SetEmail(email);
        SetPhoneNumber(phoneNumber);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetBirthDate(birthDate);
    }


    public string CredentialId { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    internal void MarkDeleted(DateTime now)
    {
        IsDeleted = true;
        DeletedAt = now;
    }

    internal void MarkCreated(DateTime now)
    {
        CreatedAt = now;
    }

    internal void MarkUpdated(DateTime now)
    {
        UpdatedAt = now;
    }


    public void SetCredentialId(string credentialId)
    {
        CredentialId = NormalizeAndValidateCredentialId(credentialId);
    }

    public void SetEmail(string email)
    {
        Email = NormalizeAndValidateEmail(email);
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = NormalizeAndValidatePhoneNumber(phoneNumber);
    }

    public void SetFirstName(string firstName)
    {
        FirstName = NormalizeAndValidateFirstName(firstName);
    }

    public void SetLastName(string lastName)
    {
        LastName = NormalizeAndValidateLastName(lastName);
    }

    public void SetBirthDate(DateOnly birthDate)
    {
        BirthDate = birthDate;
    }

    public static User Create(
        string credentialId,
        string email,
        string phoneNumber,
        string firstName,
        string lastName,
        DateOnly birthDate
    )
    {
        return new User(credentialId, email, phoneNumber, firstName, lastName, birthDate);
    }


    private static string NormalizeAndValidateCredentialId(string credentialId)
    {
        if (string.IsNullOrEmpty(credentialId))
            throw new DomainException("User credential ID is required");

        return credentialId.Trim();
    }


    private static string NormalizeAndValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new DomainException("User email is required");

        email = email.Trim();

        if (email.Length > UserConstants.EmailMaxLength)
            throw new DomainException($"User email cannot exceed {UserConstants.EmailMaxLength} characters");

        if (!Regex.IsMatch(email, UserConstants.EmailPattern))
            throw new DomainException("User email is invalid");


        return email;
    }

    private static string NormalizeAndValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            throw new DomainException("User phoneNumber is required");

        phoneNumber = phoneNumber.Trim();

        if (phoneNumber.Length != UserConstants.PhoneNumberExactLength)
            throw new ValidationException($"User phone number must have {UserConstants.PhoneNumberExactLength} digits");

        if (!Regex.IsMatch(phoneNumber, UserConstants.PhoneNumberPattern))
            throw new ValidationException("User phone number is invalid");


        return phoneNumber;
    }

    private static string NormalizeAndValidateFirstName(string firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            throw new DomainException("User firstname is required");

        firstName = firstName.Trim();

        if (firstName.Length > UserConstants.FirstNameMaxLength)
            throw new DomainException($"User firstname cannot exceed {UserConstants.FirstNameMaxLength} characters");

        return firstName;
    }

    private static string NormalizeAndValidateLastName(string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            throw new DomainException("User lastname is required");

        lastName = lastName.Trim();

        if (lastName.Length > UserConstants.LastNameMaxLength)
            throw new DomainException($"User lastname cannot exceed {UserConstants.LastNameMaxLength} characters");

        return lastName;
    }
}