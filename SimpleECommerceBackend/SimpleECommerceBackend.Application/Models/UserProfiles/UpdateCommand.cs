using MediatR;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public record UpdateCommand(
    string Email,
    string Password
) : IRequest<UpdateResult>;
