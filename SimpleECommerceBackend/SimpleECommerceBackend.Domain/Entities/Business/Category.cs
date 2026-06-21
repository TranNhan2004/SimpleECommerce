using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Constants.ValidationRules;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Category : EntityBase
{
    public Category()
    {
    }

    private string _name = null!;
    private string? _description;
    private CategoryStatus _status;

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
        set
        {
            if (_status != value)
            {
                switch (value)
                {
                    case CategoryStatus.Active:
                        Activate();
                        break;
                    case CategoryStatus.Inactive:
                        Deactivate();
                        break;
                    case CategoryStatus.Archived:
                        Archive();
                        break;
                }
            }
        }
    }

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
