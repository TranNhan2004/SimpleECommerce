using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

[AutoConstructor]
public partial class UserContextHolder : IUserContextHolder
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IUserContext GetUserContext()
    {
        if (TryGet(out var userContext) && userContext is not null)
            return userContext;

        throw new UnauthorizedException(
            CurrentUserErrorCode.Unauthenticated,
            "Current user is not authenticated."
        );
    }

    public bool TryGet(out IUserContext? userContext)
    {
        userContext = null;

        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return false;

        var rawUserId = FindFirstValue(principal, "sub");
        if (string.IsNullOrWhiteSpace(rawUserId))
            throw new UnauthorizedException(
                CurrentUserErrorCode.UserIdMissing,
                "Current user id claim 'sub' is missing.",
                new Dictionary<string, object?>
                {
                    ["field"] = "User"
                }
            );

        if (!Guid.TryParse(rawUserId, out var userId))
            throw new UnauthorizedException(
                CurrentUserErrorCode.UserIdInvalid,
                $"Current user id claim 'sub' is invalid: {rawUserId}.",
                new Dictionary<string, object?>
                {
                    ["field"] = "User",
                    ["value"] = rawUserId
                }
            );

        if (!TryResolveRole(principal, out var role))
            throw new ForbiddenException(
                CurrentUserErrorCode.RoleMissing,
                "Current user role claim is missing or invalid.",
                new Dictionary<string, object?>
                {
                    ["field"] = "Role"
                }
            );

        var email = FindFirstValue(principal, ClaimTypes.Email)
                    ?? FindFirstValue(principal, "email")
                    ?? FindFirstValue(principal, "preferred_username")
                    ?? string.Empty;

        userContext = new UserContext(userId, email, role);
        return true;
    }

    private static bool TryResolveRole(ClaimsPrincipal principal, out Role role)
    {
        foreach (var roleName in principal.FindAll("roles").Select(claim => claim.Value)
                     .Concat(principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (RoleUtils.TryParse(roleName, out role))
                return true;
        }

        role = default;
        return false;
    }

    private static string? FindFirstValue(ClaimsPrincipal principal, string claimType)
    {
        return principal.FindFirst(claimType)?.Value;
    }

    private sealed record UserContext(Guid Id, string Email, Role Role) : IUserContext;
}
