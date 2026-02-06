using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Auth;

public class Permission : EntityBase
{
    private Permission()
    {
    }

    private Permission(string name, string? description)
    {
        SetName(name);
        SetDescription(description);
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Role name cannot be empty");

        var trimmedName = name.Trim();

        if (trimmedName.Length > RoleConstants.NameMaxLength)
            throw new DomainException($"Role name cannot exceed {RoleConstants.NameMaxLength} characters");

        Name = trimmedName;
    }

    public void SetDescription(string? description)
    {
        if (description is null)
        {
            Description = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Description is not blank");

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > CategoryConstants.DescriptionMaxLength)
            throw new DomainException(
                $"Description cannot exceed {CategoryConstants.DescriptionMaxLength} characters");

        Description = trimmedDescription;
    }

    public static Permission Create(string name, string? description)
    {
        return new Permission(name, description);
    }
}