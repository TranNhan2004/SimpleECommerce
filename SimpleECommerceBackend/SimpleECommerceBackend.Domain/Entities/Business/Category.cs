using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Category : EntityBase, IAuditable, ICreationActorTrackable, IUpdateActorTrackable
{
    private Category()
    {
    }

    private Category(string name, string? description)
    {
        SetName(name);
        SetDescription(description);
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Guid CreatedBy { get; private set; }

    public void MarkCreatedBy(Guid actorId)
    {
        if (actorId == Guid.Empty)
            throw new DomainException("Actor ID is required");

        if (CreatedBy != Guid.Empty)
            throw new DomainException("Creation actor of this category has already been set");

        CreatedBy = actorId;
    }

    public Guid? UpdatedBy { get; private set; }

    public void MarkUpdatedBy(Guid actorId)
    {
        if (actorId == Guid.Empty)
            throw new DomainException("Actor ID is required");

        UpdatedBy = actorId;
    }


    public static Category Create(string name, string? description)
    {
        return new Category(name, description);
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required");

        var trimmedName = name.Trim();

        if (trimmedName.Length > CategoryConstants.NameMaxLength)
            throw new DomainException($"Category name cannot exceed {CategoryConstants.NameMaxLength} characters");

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
            throw new DomainException("Category description is not blank");

        var trimmedDescription = description.Trim();

        if (trimmedDescription.Length > CategoryConstants.DescriptionMaxLength)
            throw new DomainException(
                $"Category description cannot exceed {CategoryConstants.DescriptionMaxLength} characters");

        Description = trimmedDescription;
    }
}