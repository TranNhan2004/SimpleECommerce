using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.ValueObjects;

namespace SimpleECommerceBackend.Application.UseCases.Catalog.Commands.UpdateProductPrice;

public record UpdateProductPriceCommand(
    Guid ProductId,
    decimal Amount,
    string Currency
) : IRequest;

[AutoConstructor]
public partial class UpdateProductPriceHandler : IRequestHandler<UpdateProductPriceCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductPriceRepository _productPriceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.ProductId);
        if (product is null)
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");

        var money = new Money(request.Amount, request.Currency);
        
        // Update product current price shortcut
        product.SetCurrentPrice(money);
        _productRepository.Update(product);

        // Add to history
        var newPrice = ProductPrice.Create(
            product.Id,
            money,
            DateTime.UtcNow
        );
        _productPriceRepository.Add(newPrice);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
