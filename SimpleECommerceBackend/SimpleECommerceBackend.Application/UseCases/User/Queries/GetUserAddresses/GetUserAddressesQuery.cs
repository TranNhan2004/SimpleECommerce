using MediatR;
using SimpleECommerceBackend.Application.UseCases.User.DTOs;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.User.Queries.GetUserAddresses;

public record GetUserAddressesQuery(Guid UserId) : IRequest<IEnumerable<UserAddressDto>>;

[AutoConstructor]
public partial class GetUserAddressesHandler : IRequestHandler<GetUserAddressesQuery, IEnumerable<UserAddressDto>>
{
    private readonly IUserShippingAddressRepository _userShippingAddressRepository;

    public async Task<IEnumerable<UserAddressDto>> Handle(GetUserAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await _userShippingAddressRepository.FindByCustomerIdAsync(request.UserId);

        return addresses.Select(a => new UserAddressDto(
            a.Id,
            a.RecipientName,
            a.PhoneNumber,
            a.AddressLine,
            a.Ward,
            a.Province,
            a.IsDefault
        ));
    }
}
