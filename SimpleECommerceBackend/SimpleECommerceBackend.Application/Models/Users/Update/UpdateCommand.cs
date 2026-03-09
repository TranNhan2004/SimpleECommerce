using MediatR;

namespace SimpleECommerceBackend.Application.Models.Users.Update;

public record UpdateCommand(
    string Email,
    string Password
) : IRequest<UpdateResult>;
