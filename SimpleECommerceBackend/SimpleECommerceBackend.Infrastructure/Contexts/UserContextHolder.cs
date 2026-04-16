using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

[AutoConstructor]
public partial class UserContextHolder : IUserContextHolder
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserProfileService _userProfileService;

    public IUserContext GetUserContext()
    {
        if (TryGet(false, out var userContext) && userContext is not null)
            return userContext;

        throw new UnauthorizedException(
            CurrentUserErrorCode.Unauthenticated,
            "Current user is not authenticated."
        );
    }

    public IUserContext GetActiveUserContext()
    {
        if (TryGet(true, out var userContext) && userContext is not null)
            return userContext;

        throw new UnauthorizedException(
            CurrentUserErrorCode.Unauthenticated,
            "Current user is not authenticated."
        );
    }

    public void ThrowIfNoActiveUserContext()
    {
        if (!TryGet(true, out var userContext) || userContext is null)
            throw new UnauthorizedException(
                CurrentUserErrorCode.Unauthenticated,
                "Current user is not authenticated."
            );
    }

    private bool TryGet(bool isActiveUser, out IUserContext? userContext)
    {
        userContext = null;

        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return false;

        var rawUserId = KeycloakPayloadExtractionHelper.FindUserId(principal);
        if (string.IsNullOrWhiteSpace(rawUserId))
            throw new UnauthorizedException(
                CurrentUserErrorCode.UserIdMissing,
                "Current user id claim 'sub' is missing.",
                new Dictionary<string, object?>
                {
                    ["field"] = "User"
                }
            );

        if (isActiveUser)
        {
            var isActive = _userProfileService.IsActiveUserAsync(Guid.Parse(rawUserId)).GetAwaiter().GetResult();
            if (!isActive)
                throw new UnauthorizedException(
                    UserProfileErrorCode.InactiveUser,
                    "Current user is not active."
                );
        }

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

        var email = KeycloakPayloadExtractionHelper.FindEmail(principal) ?? string.Empty;

        userContext = new UserContext(userId, email, role);
        return true;
    }

    private static bool TryResolveRole(ClaimsPrincipal principal, out Role role)
    {
        foreach (var roleName in KeycloakPayloadExtractionHelper.GetRoleNames(principal))
        {
            if (RoleUtils.TryParse(roleName, out role))
                return true;
        }

        role = default;
        return false;
    }

    private sealed record UserContext(Guid Id, string Email, Role Role) : IUserContext;
}
