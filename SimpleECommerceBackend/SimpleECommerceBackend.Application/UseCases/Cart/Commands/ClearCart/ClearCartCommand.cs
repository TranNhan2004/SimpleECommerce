using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Cart.Commands.ClearCart;

public record ClearCartCommand(Guid UserId) : IRequest;

[AutoConstructor]
public partial class ClearCartHandler : IRequestHandler<ClearCartCommand>
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        // Repo needs DeleteAllByCustomerId? Or fetch and delete loop?
        // ICartItemRepository usually has FindByCustomerIdAsync.
        var items = await _cartItemRepository.FindByCustomerIdAsync(request.UserId);
        
        foreach (var item in items)
        {
            _cartItemRepository.Delete(item);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
