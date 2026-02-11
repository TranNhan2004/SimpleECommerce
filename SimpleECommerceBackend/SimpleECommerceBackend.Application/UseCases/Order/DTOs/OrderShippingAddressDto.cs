namespace SimpleECommerceBackend.Application.UseCases.Order.DTOs;

public record OrderShippingAddressDto(
    string RecipientName,
    string PhoneNumber,
    string AddressLine,
    string? Ward,
    string? Province
);
