using SimpleECommerceBackend.Application.Results;

namespace SimpleECommerceBackend.Application.UseCases.Role;

public static class RoleErrors
{
    public static readonly Error NotFound =
        new("NOT_FOUND", "Role not found");

    public static readonly Error NameAlreadyExists =
        new("AUTH_EMAIL_EXISTS", "Name already exists");
}