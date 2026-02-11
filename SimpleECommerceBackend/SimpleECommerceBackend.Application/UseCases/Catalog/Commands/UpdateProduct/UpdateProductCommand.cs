using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Catalog.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    ProductStatus Status
) : IRequest;

[AutoConstructor]
public partial class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id);

        if (product is null)
        {
            // Throw exception or return result? Domain exception or Application exception?
            // For simplicity, throwing generic Exception or DomainException if appropriate.
            // Or better, KeyNotFoundException.
            throw new KeyNotFoundException($"Product with ID {request.Id} not found");
        }

        product.SetName(request.Name);
        product.SetDescription(request.Description);
        product.SetCategoryId(request.CategoryId);
        product.SetStatus(request.Status);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
