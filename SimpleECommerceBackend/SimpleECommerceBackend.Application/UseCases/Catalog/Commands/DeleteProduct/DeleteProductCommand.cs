using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Interfaces.Time;

namespace SimpleECommerceBackend.Application.UseCases.Catalog.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;

[AutoConstructor]
public partial class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock; // Need clock for soft delete?
    // Product entity has ISoftDeletable? Yes. 
    // Does it have a SoftDelete method?
    // Need to check Product.cs. If using interface, I can use property.

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id);
        if (product is null)
             // Already deleted or not found. Idempotent success? Or throw?
             // Throw for now to be explicit.
             throw new KeyNotFoundException($"Product with ID {request.Id} not found");

        // Assuming SoftDelete method exists or ISoftDeletable properties are settable.
        // Product.cs Step 298: Check for SoftDelete method.
        // It has `public class Product : EntityBase, ... ISoftDeletable`
        // But I didn't see explicit SoftDelete method in Step 298.
        // I might need to add it or cast to ISoftDeletable if setters are public or internal.
        // Let's assume I cast to interface or add method.
        // Wait, Step 298 showed `Product` class. I didn't see `SoftDelete`.
        // I will add `SoftDelete` method to `Product` via file replacement if needed, 
        // OR use the interface properties if approachable.
        // `ISoftDeletable` usually has `IsDeleted` and `DeletedAt`.
        // Let's modify `Product.cs` to add `SoftDelete` if missing, or use strictly.
        // I'll try to use interface.
        
        // However, standard DDD pattern implies a method on entity.
        // I'll assume I can add it or it exists in `EntityBase`? No, `EntityBase` usually just Id.
        
        // I'll update Product.cs to add SoftDelete method if I can't find it.
        // But for now, I'll rely on property setting if public.
        // Actually, Step 298:
        // `public DateTimeOffset CreatedAt { get; private set; }`
        // `public DateTimeOffset? UpdatedAt { get; private set; }`
        // `public bool IsDeleted { get; private set; }` (implied by interface?)
        // Step 298 didn't show `ISoftDeletable` implementation details (properties) explicitly in the snippet?
        // Ah, `public class Product : EntityBase, ICreatedTime, IUpdatedTime`
        // It DOES NOT implement `ISoftDeletable` in Step 298!
        // Wait, `implementation_plan.md` says "Product - Products with pricing, status, soft delete".
        // Let me re-read `Product.cs` Step 298 carefully.
        // Line 10: `public class Product : EntityBase, ICreatedTime, IUpdatedTime`
        // It does NOT implement `ISoftDeletable`.
        // But `CustomerShippingAddress` in Step 161 DOES implement `ISoftDeletable`.
        
        // Config `ProductConfiguration.cs` Step 223:
        // `builder.Property(p => p.IsDeleted)...`
        // This implies Product class HAS `IsDeleted`.
        // Maybe Step 298 snippet was old or I missed it?
        // Step 28 was `Product.cs` view.
        // Line 10: `public class Product : EntityBase, ICreatedTime, IUpdatedTime, ISoftDeletable`
        // Ah! Step 28 shows it DOES implement `ISoftDeletable`. Step 298 snippet might have been truncated or I misread.
        // Step 298: "Showing lines 1 to 109". Line 10: `public class Product : EntityBase, ICreatedTime, IUpdatedTime`
        // IT IS MISSING `ISoftDeletable` in Step 298!
        // But Step 28 `Product.cs` had it!
        // Step 28: `10: public class Product : EntityBase, ICreatedTime, IUpdatedTime, ISoftDeletable`
        
        // This is conflicting. Why did it disappear?
        // Maybe I overwrote it? I didn't overwrite `Product.cs`. I only viewed it.
        // Wait, Step 28 was BEFORE I did anything. Step 298 is NOW.
        // Did I view the same file?
        // Step 28: `SimpleECommerceBackend.Domain/Entities/Business/Product.cs`
        // Step 298: `SimpleECommerceBackend.Domain/Entities/Business/Product.cs`
        
        // Ah, in Step 28, I saw the file content.
        // In Step 298, I saw the file content again.
        // Line 1 to 109.
        // Line 10 in Step 298: `public class Product : EntityBase, ICreatedTime, IUpdatedTime`
        // It seems `ISoftDeletable` is MISSING in Step 298.
        // BUT `ProductConfiguration` (Step 223) references `IsDeleted`.
        // If I compile, it would fail if property is missing.
        // So `Product.cs` MUST have `IsDeleted`.
        // Maybe I accidentally replaced it? I haven't replaced `Product.cs`.
        // I created `ProductRepository`, `ProductImageRepository`, `IProductRepository`...
        // I haven't touched `Product.cs`.
        
        // Very strange. Let me check `Product.cs` content again to be absolutely sure.
        // And if needed, fix it to implement `ISoftDeletable`.
        
        // For `DeleteProductCommand`, I will assume `SoftDelete` method exists or I'll implement it.
        // `CustomerShippingAddress` has `SoftDelete(IClock clock)`. I should follow that pattern.
        
        // I'll call `product.SoftDelete(_clock);` in Handler.
        // If compilation fails, I'll fix Product entity.
        
        // Wait, I need to check `ProductPriceRepository` first as planned.
        
        // I'll assume `SoftDelete` exists for now code-wise.
         // product.SoftDelete(_clock); // Casting or calling method
         // _productRepository.Delete(product); // Repo update
         // await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
