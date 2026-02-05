using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Category : EntityBase, ICreatedTime, IUpdatedTime
{
    private Category()
    {
    }

    private Category(string name, string? description, Guid adminId)
    {
        SetName(name);
        SetDescription(description);
        SetAdminId(adminId);
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public Guid AdminId { get; private set; }
    public UserProfile? Admin { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }


    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required");

        var trimmedName = name.Trim();

        if (trimmedName.Length > CategoryConstants.NameMaxLength)
            throw new DomainException($"Name cannot exceed {CategoryConstants.NameMaxLength} characters");

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

    public void SetAdminId(Guid adminId)
    {
        if (adminId == Guid.Empty)
            throw new DomainException("Admin is required");

        AdminId = adminId;
    }

    public static Category Create(string name, string? description, Guid adminId)
    {
        return new Category(name, description, adminId);
    }
}