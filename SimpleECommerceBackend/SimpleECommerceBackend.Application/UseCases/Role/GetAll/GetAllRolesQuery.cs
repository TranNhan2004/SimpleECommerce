using MediatR;
using SimpleECommerceBackend.Application.Results;

namespace SimpleECommerceBackend.Application.UseCases.Role.GetAll;

public sealed record GetAllRolesQuery : IRequest<Result<GetAllRolesResult>>;