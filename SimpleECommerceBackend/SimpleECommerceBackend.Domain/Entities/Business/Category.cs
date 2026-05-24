using System.Text.Json.Serialization;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Category : EntityBase, ICreatedTrackable, IUpdatedTrackable
{
    public Category()
    {
    }

    // [JsonConstructor]
    // private Category(
    //     Guid id,
    //     string name,
    //     string? description,
    //     CategoryStatus status,
    //     Guid adminId,
    //     DateTimeOffset createdAt,
    //     DateTimeOffset? updatedAt
    // )
    // {
    //     Id = id;
    //     Name = name;
    //     Description = description;
    //     Status = status;
    //     AdminId = adminId;
    //     CreatedAt = createdAt;
    //     UpdatedAt = updatedAt;
    // }

    private Category(string name, string? description, CategoryStatus status, Guid adminId)
    {
        Id = UuidUtils.CreateV7();
        Name = name;
        Description = description;
        Status = status;
        AdminId = adminId;
    }

    private string _name = null!;
    private string? _description;
    private CategoryStatus _status;
    private Guid _adminId;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    CategoryErrorCodes.NameRequired,
                    "Name is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Name"
                    }
                );

            var trimmedName = value.Trim();

            if (trimmedName.Length > CategoryValidationRules.NameMaxLength)
                throw new ValidationException(
                    CategoryErrorCodes.NameMaxLengthExceeded,
                    $"Name cannot exceed {CategoryValidationRules.NameMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Name",
                        ["max"] = CategoryValidationRules.NameMaxLength
                    }
                );

            _name = trimmedName;
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (value is null)
            {
                _description = null;
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    CategoryErrorCodes.DescriptionMustNotBeBlank,
                    "Description is not blank",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description"
                    }
                );

            var trimmedDescription = value.Trim();

            if (trimmedDescription.Length > CategoryValidationRules.DescriptionMaxLength)
                throw new ValidationException(
                    CategoryErrorCodes.DescriptionMaxLengthExceeded,
                    $"Description cannot exceed {CategoryValidationRules.DescriptionMaxLength} characters",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Description",
                        ["max"] = CategoryValidationRules.DescriptionMaxLength
                    }
                );

            _description = trimmedDescription;
        }
    }

    public CategoryStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public Guid AdminId
    {
        get => _adminId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    CategoryErrorCodes.AdminRequired,
                    "Admin is required",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "Admin"
                    }
                );

            _adminId = value;
        }
    }

    public User? Admin { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public void Activate()
    {
        if (Status == CategoryStatus.Archived)
            throw new ValidationException(
                CategoryErrorCodes.ActivateNotAllowed,
                "Archived category cannot be activated",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Activate",
                    ["allowedStates"] = "not archived"
                }
            );

        if (Status != CategoryStatus.Active)
            Status = CategoryStatus.Active;
    }

    public void Deactivate()
    {
        if (Status == CategoryStatus.Archived)
            throw new ValidationException(
                CategoryErrorCodes.DeactivateNotAllowed,
                "Archived category cannot be deactivated",
                new Dictionary<string, object?>
                {
                    ["field"] = "Status",
                    ["operation"] = "Deactivate",
                    ["allowedStates"] = "not archived"
                }
            );

        if (Status != CategoryStatus.Inactive)
            Status = CategoryStatus.Inactive;
    }

    public void Archive()
    {
        if (Status == CategoryStatus.Archived)
            throw new ValidationException(
                CategoryErrorCodes.AlreadyArchived,
                "Category already archived",
                new Dictionary<string, object?>
                {
                    ["field"] = "Category"
                }
            );

        Status = CategoryStatus.Archived;
    }
}
