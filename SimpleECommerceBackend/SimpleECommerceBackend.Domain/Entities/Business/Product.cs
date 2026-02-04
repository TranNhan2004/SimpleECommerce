using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces.Entities;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities.Business;

public class Product : EntityBase, IAuditable, ISoftDeletable, ICreationActorTrackable, IUpdateActorTrackable
{
    private Product()
    {
    }

    private Product(string name, string description, Money price, Guid categoryId)
    {
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money CurrentPrice { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }


    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }

    public void MarkCreatedBy(Guid actorId)
    {
        if (actorId == Guid.Empty)
            throw new DomainException("Actor ID is required");

        if (CreatedBy != Guid.Empty)
            throw new DomainException("Creation actor of this product has already been set");

        CreatedBy = actorId;
    }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public void SoftDelete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public Guid? UpdatedBy { get; private set; }

    public void MarkUpdatedBy(Guid actorId)
    {
        if (actorId == Guid.Empty)
            throw new DomainException("Actor ID is required");

        UpdatedBy = actorId;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");

        var trimmedName = name.Trim();
        if (trimmedName.Length > ProductConstants.NameMaxLength)
            throw new DomainException($"Product name cannot exceed {ProductConstants.NameMaxLength} characters");

        Name = trimmedName;
    }

    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Product description is required");

        var trimmedDescription = description.Trim();
        if (trimmedDescription.Length > ProductConstants.DescriptionMaxLength)
            throw new DomainException(
                $"Product description cannot exceed {ProductConstants.DescriptionMaxLength} characters");

        Description = trimmedDescription;
    }
}