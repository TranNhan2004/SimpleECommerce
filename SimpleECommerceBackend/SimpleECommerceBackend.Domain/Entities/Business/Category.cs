using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces;

namespace SimpleECommerceBackend.Domain.Entities;

public class Category : EntityBase, IAuditable, ICreatedByUser, IUpdatedByUser
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
    public Guid? UpdatedBy { get; private set; }

    internal void MarkCreated(DateTime now)
    {
        CreatedAt = now;
    }

    internal void MarkUpdated(DateTime now)
    {
        UpdatedAt = now;
    }

    internal void MarkCreatedBy(Guid userId)
    {
        CreatedBy = userId;
    }

    internal void MarkUpdatedBy(Guid userId)
    {
        UpdatedBy = userId;
    }

    public static Category Create(string name, string? description)
    {
        return new Category(name, description);
    }

    public void SetName(string name)
    {
        Name = NormalizeAndValidateName(name);
    }

    public void SetDescription(string? description)
    {
        Description = NormalizeAndValidateDescription(description);
    }

    public void SetCreatedBy(Guid createdBy)
    {
        CreatedBy = createdBy;
    }

    public void SetUpdatedBy(Guid updatedBy)
    {
        UpdatedBy = updatedBy;
    }

    private static string NormalizeAndValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required");

        name = name.Trim();

        if (name.Length > CategoryConstants.NameMaxLength)
            throw new DomainException($"Category name cannot exceed {CategoryConstants.NameMaxLength} characters");

        return name;
    }

    private static string? NormalizeAndValidateDescription(string? description)
    {
        if (description is null)
            return null;

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Category description is not blank");

        description = description.Trim();

        if (description.Length > CategoryConstants.DescriptionMaxLength)
            throw new DomainException(
                $"Category description cannot exceed {CategoryConstants.DescriptionMaxLength} characters");

        return description;
    }
}