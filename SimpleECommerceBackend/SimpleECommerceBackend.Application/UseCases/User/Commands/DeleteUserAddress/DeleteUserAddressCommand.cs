using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Interfaces.Time;

namespace SimpleECommerceBackend.Application.UseCases.User.Commands.DeleteUserAddress;

public record DeleteUserAddressCommand(Guid UserId, Guid AddressId) : IRequest;

[AutoConstructor]
public partial class DeleteUserAddressHandler : IRequestHandler<DeleteUserAddressCommand>
{
    private readonly IUserShippingAddressRepository _userShippingAddressRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public async Task Handle(DeleteUserAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _userShippingAddressRepository.FindByIdAsync(request.AddressId);
        if (address is null || address.CustomerId != request.UserId)
             throw new KeyNotFoundException("Address not found");

        // Logic: if default, prevent delete? Or force users to choose new default?
        // Or just allow.
        
        address.SoftDelete(_clock);
        _userShippingAddressRepository.Update(address); // Soft delete update
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
