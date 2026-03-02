using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Category : IEntity, ICreatedTrackable, IUpdatedTrackable
{
    private Category()
    {
    }

    private Category(string name, string? description, CategoryStatus status, Guid adminId)
    {
        SetName(name);
        SetDescription(description);
        SetStatus(status);
        SetAdminId(adminId);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public CategoryStatus Status { get; private set; }
    public Guid AdminId { get; private set; }
    public UserProfile? Admin { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }


    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException("Name is required");

        var trimmedName = name.Trim();

        if (trimmedName.Length > CategoryConstants.NameMaxLength)
            throw new BusinessException($"Name cannot exceed {CategoryConstants.NameMaxLength} characters");

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
            throw new BusinessException("Description is not blank");

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > CategoryConstants.DescriptionMaxLength)
            throw new BusinessException(
                $"Description cannot exceed {CategoryConstants.DescriptionMaxLength} characters");

        Description = trimmedDescription;
    }

    private void SetStatus(CategoryStatus status)
    {
        Status = status;
    }

    public void Activate()
    {
        if (Status == CategoryStatus.Archived)
            throw new BusinessException("Archived category cannot be activated");

        if (Status != CategoryStatus.Active)
            SetStatus(CategoryStatus.Active);
    }

    public void Deactivate()
    {
        if (Status == CategoryStatus.Archived)
            throw new BusinessException("Archived category cannot be deactivated");

        if (Status != CategoryStatus.Inactive)
            SetStatus(CategoryStatus.Inactive);
    }

    public void Archive()
    {
        if (Status == CategoryStatus.Archived)
            throw new BusinessException("Category already archived");

        SetStatus(CategoryStatus.Archived);
    }

    public void SetAdminId(Guid adminId)
    {
        if (adminId == Guid.Empty)
            throw new BusinessException("Admin is required");

        AdminId = adminId;
    }

    public static Category Create(string name, string? description, Guid adminId)
    {
        return new Category(name, description, CategoryStatus.Active, adminId);
    }
}