using SimpleECommerceBackend.Api.Dtos.Common.Errors;
using SimpleECommerceBackend.Api.Factories;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Api.Middleware;

public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly IStaticTextLocalizer _staticTextLocalizer;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment,
        IStaticTextLocalizer staticTextLocalizer
    )
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _staticTextLocalizer = staticTextLocalizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        LogException(exception);

        var (statusCode, errorResponse) = MapExceptionToResponse(context, exception);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private void LogException(Exception exception)
    {
        if (exception is ExceptionBase ex)
        {
            _logger.LogError(
                ex,
                "Handled exception occurred: {ExceptionType} - {Message}",
                ex.GetType().Name,
                ex.InternalMessage
            );
        }
        else
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred: {ExceptionType} - {Message}",
                exception.GetType().Name,
                exception.Message
            );
        }
    }

    private (int StatusCode, ErrorResponse Response) MapExceptionToResponse(
        HttpContext context,
        Exception exception
    )
    {
        return exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status422UnprocessableEntity,
                ErrorResponseFactory.CreateFromException(
                    context,
                    validationException,
                    StatusCodes.Status422UnprocessableEntity,
                    "Problem.Validation",
                    "https://tools.ietf.org/html/rfc4918#section-11.2",
                    _environment,
                    _staticTextLocalizer
                )
            ),

            ResourceNotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                ErrorResponseFactory.CreateFromException(
                    context,
                    notFoundException,
                    StatusCodes.Status404NotFound,
                    "Problem.NotFound",
                    "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    _environment,
                    _staticTextLocalizer
                )
            ),

            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                ErrorResponseFactory.CreateFromException(
                    context,
                    conflictException,
                    StatusCodes.Status409Conflict,
                    "Problem.Conflict",
                    "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    _environment,
                    _staticTextLocalizer
                )
            ),

            UnauthorizedException unauthorizedException => (
                StatusCodes.Status401Unauthorized,
                ErrorResponseFactory.CreateFromException(
                    context,
                    unauthorizedException,
                    StatusCodes.Status401Unauthorized,
                    "Problem.Unauthorized",
                    "https://tools.ietf.org/html/rfc7235#section-3.1",
                    _environment,
                    _staticTextLocalizer
                )
            ),

            ForbiddenException forbiddenException => (
                StatusCodes.Status403Forbidden,
                ErrorResponseFactory.CreateFromException(
                    context,
                    forbiddenException,
                    StatusCodes.Status403Forbidden,
                    "Problem.Forbidden",
                    "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    _environment,
                    _staticTextLocalizer
                )
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                ErrorResponseFactory.CreateUnexpected(context, exception, _environment, _staticTextLocalizer)
            )
        };
    }
}
