using SimpleECommerceBackend.Domain.Constants.Auth;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Auth;

public class Role : EntityBase
{
    private Role()
    {
    }

    private Role(string name)
    {
        SetName(name);
    }

    public string Name { get; set; } = string.Empty;

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Role name cannot be empty");

        var trimmedName = name.Trim();

        if (trimmedName.Length > RoleConstants.NameMaxLength)
            throw new DomainException($"Role name cannot exceed {RoleConstants.NameMaxLength} characters");

        Name = trimmedName;
    }

    public static Role Create(string name)
    {
        return new Role(name);
    }
}