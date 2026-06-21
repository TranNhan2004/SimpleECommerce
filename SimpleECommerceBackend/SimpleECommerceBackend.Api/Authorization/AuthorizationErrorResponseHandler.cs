using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using SimpleECommerceBackend.Api.Factories;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;

namespace SimpleECommerceBackend.Api.Authorization;

public sealed class AuthorizationErrorResponseHandler : IAuthorizationMiddlewareResultHandler
{
    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult
    )
    {
        if (authorizeResult.Succeeded)
        {
            await next(context);
            return;
        }

        var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
        var staticTextLocalizer = context.RequestServices.GetRequiredService<IStaticTextLocalizer>();

        if (authorizeResult.Challenged)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status401Unauthorized,
                "Problem.Unauthorized",
                "https://tools.ietf.org/html/rfc7235#section-3.1",
                AuthorizationErrorCodes.Unauthorized,
                environment,
                staticTextLocalizer
            );
            return;
        }

        if (authorizeResult.Forbidden)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status403Forbidden,
                "Problem.Forbidden",
                "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                AuthorizationErrorCodes.Forbidden,
                environment,
                staticTextLocalizer
            );
            return;
        }

        await next(context);
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        int statusCode,
        string titleKey,
        string type,
        string errorCode,
        IHostEnvironment environment,
        IStaticTextLocalizer staticTextLocalizer
    )
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var errorResponse = ErrorResponseFactory.Create(
            context,
            statusCode,
            titleKey,
            type,
            errorCode,
            environment,
            staticTextLocalizer
        );

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}
