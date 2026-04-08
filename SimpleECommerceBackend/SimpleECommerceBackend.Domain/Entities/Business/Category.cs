using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Category : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private Category()
    {
    }

    private Category(string name, string? description, CategoryStatus status, Guid adminId)
    {
        SetId(Guid.NewGuid());
        SetName(name);
        SetDescription(description);
        SetStatus(status);
        SetAdminId(adminId);
    }

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
            throw new ValidationException(
                CategoryErrorCode.NameRequired,
                "Name is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Name"
                }
            );

        var trimmedName = name.Trim();

        if (trimmedName.Length > CategoryConstants.NameMaxLength)
            throw new ValidationException(
                CategoryErrorCode.NameMaxLengthExceeded,
                $"Name cannot exceed {CategoryConstants.NameMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Name",
                    ["max"] = CategoryConstants.NameMaxLength
                }
            );

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
            throw new ValidationException(
                CategoryErrorCode.DescriptionMustNotBeBlank,
                "Description is not blank",
                new Dictionary<string, object?>
                {
                    ["field"] = "Description"
                }
            );

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > CategoryConstants.DescriptionMaxLength)
            throw new ValidationException(
                CategoryErrorCode.DescriptionMaxLengthExceeded,
                $"Description cannot exceed {CategoryConstants.DescriptionMaxLength} characters",
                new Dictionary<string, object?>
                {
                    ["field"] = "Description",
                    ["max"] = CategoryConstants.DescriptionMaxLength
                }
            );

        Description = trimmedDescription;
    }

    private void SetStatus(CategoryStatus status)
    {
        Status = status;
    }

    public void Activate()
    {
        if (Status == CategoryStatus.Archived)
            throw new ValidationException(
                CategoryErrorCode.ActivateNotAllowed,
                "Archived category cannot be activated",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Activate",
                    ["allowedStates"] = "not archived"
                }
            );

        if (Status != CategoryStatus.Active)
            SetStatus(CategoryStatus.Active);
    }

    public void Deactivate()
    {
        if (Status == CategoryStatus.Archived)
            throw new ValidationException(
                CategoryErrorCode.DeactivateNotAllowed,
                "Archived category cannot be deactivated",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Deactivate",
                    ["allowedStates"] = "not archived"
                }
            );

        if (Status != CategoryStatus.Inactive)
            SetStatus(CategoryStatus.Inactive);
    }

    public void Archive()
    {
        if (Status == CategoryStatus.Archived)
            throw new ValidationException(
                CategoryErrorCode.AlreadyArchived,
                "Category already archived",
                new Dictionary<string, object?>
                {
                    ["field"] = "Category"
                }
            );

        SetStatus(CategoryStatus.Archived);
    }

    public void SetAdminId(Guid adminId)
    {
        if (adminId == Guid.Empty)
            throw new ValidationException(
                CategoryErrorCode.AdminRequired,
                "Admin is required",
                new Dictionary<string, object?>
                {
                    ["field"] = "Admin"
                }
            );

        AdminId = adminId;
    }

    public static Category Create(string name, string? description, Guid adminId)
    {
        return new Category(name, description, CategoryStatus.Active, adminId);
    }
}