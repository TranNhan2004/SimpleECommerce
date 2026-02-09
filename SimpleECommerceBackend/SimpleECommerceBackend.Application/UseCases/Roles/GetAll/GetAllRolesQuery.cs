using MediatR;

namespace SimpleECommerceBackend.Application.UseCases.Roles.GetAll;

public sealed record GetAllRolesQuery : IRequest<GetAllRolesResult>;