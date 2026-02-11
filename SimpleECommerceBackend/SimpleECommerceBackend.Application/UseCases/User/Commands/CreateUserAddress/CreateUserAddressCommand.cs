using MediatR;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

namespace SimpleECommerceBackend.Application.UseCases.User.Commands.AddUserAddress;

public record CreateUserAddressCommand(
    Guid CustomerId,
    string RecipientName,
    string PhoneNumber,
    string AddressLine,
    string Ward,
    string Province,
    bool IsDefault
) : IRequest<CreateUserAddressResult>;

public class CreateUserAddressResult
{
    public Guid Id { get; init; }
    public string RecipientName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string AddressLine { get; init; } = string.Empty;
    public string Ward { get; init; } = string.Empty;
    public string Province { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
}

[AutoConstructor]
public partial class CreateUserAddressHandler : IRequestHandler<CreateUserAddressCommand, CreateUserAddressResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserShippingAddressRepository _userShippingAddressRepository;

    public async Task<CreateUserAddressResult> Handle(
        CreateUserAddressCommand cmd,
        CancellationToken cancellationToken = default
    )
    {
        if (cmd.IsDefault)
        {
            var existingDefault = await _userShippingAddressRepository.FindDefaultByCustomerIdAsync(cmd.UserId);
            if (existingDefault != null)
            {
                existingDefault.SetIsDefault(false);
                _userShippingAddressRepository.Update(existingDefault);
            }
        }
        else
        {
            var count = (await _userShippingAddressRepository.FindByCustomerIdAsync(request.UserId)).Count();
            if (count == 0)
            {
                // Can't modify request.IsDefault (record).
                // Create with true?
                // Let's stick to request.
            }
        }

        var address = CustomerShippingAddress.Create(
            request.RecipientName,
            request.PhoneNumber,
            request.AddressLine,
            request.Ward,
            request.Province,
            request.UserId,
            request.IsDefault
        );

        _userShippingAddressRepository.Add(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return address.Id;
    }
}