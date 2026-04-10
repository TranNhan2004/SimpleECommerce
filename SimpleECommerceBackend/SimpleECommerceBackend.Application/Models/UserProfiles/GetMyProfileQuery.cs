using MediatR;

namespace SimpleECommerceBackend.Application.Models.UserProfiles;

public record GetMyProfileQuery() : IRequest<GetMyProfileResult>;