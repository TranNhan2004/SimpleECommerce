using SimpleECommerceBackend.Application.Results;

namespace SimpleECommerceBackend.Application.UseCases.Authentication;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials =
        new("AUTH_INVALID_CREDENTIALS", "Invalid email or password");

    public static readonly Error AccountInactive =
        new("AUTH_ACCOUNT_INACTIVE", "Account is not active");

    public static readonly Error EmailAlreadyExists =
        new("AUTH_EMAIL_EXISTS", "Email already exists");
}