using Microsoft.AspNetCore.Http;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private UserContext? _cachedUserContext;
    private bool _isLoaded;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public Guid Id
    {
        get => GetRequiredUserContext().Id;
    }

    public Guid KeycloakSubjectId
    {
        get => GetRequiredUserContext().KeycloakSubjectId;
    }

    public string Email
    {
        get => GetRequiredUserContext().Email;
    }

    private UserContext GetRequiredUserContext()
    {
        if (_isLoaded)
            return _cachedUserContext ?? throw CreateUnauthenticatedException();

        _cachedUserContext = LoadUserContextAsync(CancellationToken.None).GetAwaiter().GetResult();
        _isLoaded = true;
        return _cachedUserContext ?? throw CreateUnauthenticatedException();
    }

    private async Task<UserContext?> LoadUserContextAsync(CancellationToken cancellationToken)
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return null;

        var rawKeycloakSubjectId = KeycloakClaimReader.FindSubjectId(principal);
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

        var email = KeycloakClaimReader.FindEmail(principal) ?? string.Empty;
        var userId = await _userService.GetIdByKeycloakSubjectIdAsync(keycloakSubjectId);

        return new UserContext(userId, keycloakSubjectId, email);
    }

    private static UnauthorizedException CreateUnauthenticatedException()
    {
        return new UnauthorizedException(
            CurrentUserErrorCodes.Unauthenticated,
            "Current user is not authenticated."
        );
    }

    private sealed record UserContext(Guid Id, Guid KeycloakSubjectId, string Email);
}
