using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.User.Commands.SetDefaultAddress;

public record SetDefaultAddressCommand(Guid UserId, Guid AddressId) : IRequest;

[AutoConstructor]
public partial class SetDefaultAddressHandler : IRequestHandler<SetDefaultAddressCommand>
{
    private readonly IUserShippingAddressRepository _userShippingAddressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task Handle(SetDefaultAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _userShippingAddressRepository.FindByIdAsync(request.AddressId);
        if (address is null || address.CustomerId != request.UserId)
            throw new KeyNotFoundException("Address not found or access denied");

        if (address.IsDefault) return; // Already default

        var currentDefault = await _userShippingAddressRepository.FindDefaultByCustomerIdAsync(request.UserId);
        if (currentDefault != null)
        {
            currentDefault.SetIsDefault(false);
            _userShippingAddressRepository.Update(currentDefault);
        }

        address.SetIsDefault(true);
        _userShippingAddressRepository.Update(address);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
