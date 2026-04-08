using MediatR;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public record UpdateUserProfileCommand(
    string FirstName,
    string LastName,
    string? NickName,
    string Sex,
    DateOnly BirthDate
) : IRequest<UpdateUserProfileResult>;
