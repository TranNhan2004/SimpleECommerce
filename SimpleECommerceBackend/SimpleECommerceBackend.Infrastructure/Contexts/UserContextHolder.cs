using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public partial class UserContextHolder : IUserContextHolder
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextHolder(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IUserContext GetUserContext()
    {
        if (TryGet(out var userContext) && userContext is not null)
            return userContext;

        throw new UnauthorizedException(
            CurrentUserErrorCodes.Unauthenticated,
            "Current user is not authenticated."
        );
    }

    private bool TryGet(out IUserContext? userContext)
    {
        userContext = null;

        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return false;

        var rawUserId = KeycloakPayloadExtractionHelper.FindUserId(principal);
        if (string.IsNullOrWhiteSpace(rawUserId))
            throw new UnauthorizedException(
                CurrentUserErrorCodes.UserIdMissing,
                "Current user id claim 'sub' is missing.",
                new Dictionary<string, object?>
                {
                    ["field"] = "User"
                }
            );

        if (!Guid.TryParse(rawUserId, out var userId))
            throw new UnauthorizedException(
                CurrentUserErrorCodes.UserIdInvalid,
                $"Current user id claim 'sub' is invalid: {rawUserId}.",
                new Dictionary<string, object?>
                {
                    ["field"] = "User",
                    ["value"] = rawUserId
                }
            );

        var email = KeycloakPayloadExtractionHelper.FindEmail(principal) ?? string.Empty;

        userContext = new UserContext(userId, email);
        return true;
    }

    private sealed record UserContext(Guid Id, string Email) : IUserContext;
}
