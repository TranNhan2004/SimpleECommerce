using MediatR;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public record UpdateMyProfileCommand(
    string FirstName,
    string LastName,
    string? NickName,
    string Sex,
    DateOnly BirthDate
) : IRequest<UpdateMyProfileResult>;
