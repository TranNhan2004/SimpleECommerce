using SimpleECommerceBackend.Domain.Constants;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Interfaces;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Domain.Entities;

public class Product : EntityBase, IAuditable, ISoftDeletable, ICreatedByUser, IUpdatedByUser
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
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private static string NormalizeAndValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");

        if (name.Length > ProductConstants.NameMaxLength)
            throw new DomainException($"Product name cannot exceed {ProductConstants.NameMaxLength} characters");

        return name;
    }

    private static string NormalizeAndValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Product description is required");

        if (description.Length > ProductConstants.NameMaxLength)
            throw new DomainException($"Product description cannot exceed {ProductConstants.NameMaxLength} characters");

        return description;
    }
}