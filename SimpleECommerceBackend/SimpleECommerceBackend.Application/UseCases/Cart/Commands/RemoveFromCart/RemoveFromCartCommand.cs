using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.Cart.Commands.RemoveFromCart;

public record RemoveFromCartCommand(
    Guid UserId,
    Guid ProductId
) : IRequest;

[AutoConstructor]
public partial class RemoveFromCartHandler : IRequestHandler<RemoveFromCartCommand>
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _cartItemRepository.FindByIdAsync(request.ProductId, request.UserId);
        if (cartItem is null)
            return; // Idempotent

        _cartItemRepository.Delete(cartItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
