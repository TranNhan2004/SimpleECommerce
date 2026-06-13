using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public class CurrentUserContextProvider : ICurrentUserContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public CurrentUserContextProvider(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public ICurrentUserContext GetUserContext()
    {
        return GetUserContextAsync().GetAwaiter().GetResult();
    }

    public async Task<ICurrentUserContext> GetUserContextAsync(CancellationToken cancellationToken = default)
    {
        if (await TryGetAsync(cancellationToken) is { } userContext)
            return userContext;

        throw new UnauthorizedException(
            CurrentUserErrorCodes.Unauthenticated,
            "Current user is not authenticated."
        );
    }

    private async Task<ICurrentUserContext?> TryGetAsync(CancellationToken cancellationToken)
    {

        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return null;

        var rawKeycloakSubjectId = KeycloakPayloadExtractionHelper.FindKeycloakSubjectId(principal);
        if (string.IsNullOrWhiteSpace(rawKeycloakSubjectId))
        {
            throw new UnauthorizedException(
                CurrentUserErrorCodes.SubjectIdMissing,
                "Current keycloak subject id claim 'sub' is missing.",
                new Dictionary<string, object?>
                {
                    ["field"] = "User"
                }
            );
        }

        if (!Guid.TryParse(rawKeycloakSubjectId, out var keycloakSubjectId))
        {
            throw new UnauthorizedException(
               CurrentUserErrorCodes.SubjectIdInvalid,
               $"Current keycloak subject id claim 'sub' is invalid: {rawKeycloakSubjectId}.",
               new Dictionary<string, object?>
               {
                   ["field"] = "User",
                   ["value"] = rawKeycloakSubjectId
               }
           );
        }

        var email = KeycloakPayloadExtractionHelper.FindEmail(principal) ?? string.Empty;
        var userId = await _userService.GetIdByKeycloakSubjectIdAsync(keycloakSubjectId);

        return new UserContext(userId, keycloakSubjectId, email);
    }

    private sealed record UserContext(Guid Id, Guid KeycloakSubjectId, string Email) : ICurrentUserContext;
}
