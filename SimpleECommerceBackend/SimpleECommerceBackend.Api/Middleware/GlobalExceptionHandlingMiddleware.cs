using SimpleECommerceBackend.Api.DTOs.Errors;
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

        var locale = ResolveLocale(context);
        var (statusCode, errorResponse) = MapExceptionToResponse(context, exception, locale);

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
        Exception exception,
        string locale
    )
    {
        return exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status422UnprocessableEntity,
                CreateErrorResponse(
                    context,
                    validationException,
                    StatusCodes.Status422UnprocessableEntity,
                    "Problem.Validation",
                    "https://tools.ietf.org/html/rfc4918#section-11.2",
                    locale
                )
            ),

            ResourceNotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                CreateErrorResponse(
                    context,
                    notFoundException,
                    StatusCodes.Status404NotFound,
                    "Problem.NotFound",
                    "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    locale
                )
            ),

            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                CreateErrorResponse(
                    context,
                    conflictException,
                    StatusCodes.Status409Conflict,
                    "Problem.Conflict",
                    "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    locale
                )
            ),

            UnauthorizedException unauthorizedException => (
                StatusCodes.Status401Unauthorized,
                CreateErrorResponse(
                    context,
                    unauthorizedException,
                    StatusCodes.Status401Unauthorized,
                    "Problem.Unauthorized",
                    "https://tools.ietf.org/html/rfc7235#section-3.1",
                    locale
                )
            ),

            ForbiddenException forbiddenException => (
                StatusCodes.Status403Forbidden,
                CreateErrorResponse(
                    context,
                    forbiddenException,
                    StatusCodes.Status403Forbidden,
                    "Problem.Forbidden",
                    "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    locale
                )
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = _staticTextLocalizer.LocalizeProblemTitle("Problem.Unexpected", locale),
                    Status = StatusCodes.Status500InternalServerError,
                    Message = exception.Message,
                    Instance = context.Request.Path,
                    TraceId = _environment.IsDevelopment() ? context.TraceIdentifier : null,
                    Extensions = new Dictionary<string, object>
                    {
                        ["locale"] = locale
                    }
                }
            )
        };
    }

    private ErrorResponse CreateErrorResponse(
        HttpContext context,
        ExceptionBase exception,
        int statusCode,
        string titleKey,
        string type,
        string locale
    )
    {
        var localizedError = _staticTextLocalizer.LocalizeError(exception.ErrorCode, exception.Details, locale);
        var extensions = new Dictionary<string, object>
        {
            ["errorCode"] = exception.ErrorCode,
            ["locale"] = locale
        };

        if (exception.Details is not null)
            extensions["details"] = exception.Details;

        if (!string.IsNullOrWhiteSpace(localizedError.FieldKey))
            extensions["field"] = localizedError.FieldKey;

        if (!string.IsNullOrWhiteSpace(localizedError.FieldDisplayName))
            extensions["fieldDisplayName"] = localizedError.FieldDisplayName;

        if (_environment.IsDevelopment() && !string.IsNullOrWhiteSpace(exception.InternalMessage))
            extensions["internalMessage"] = exception.InternalMessage;

        return new ErrorResponse
        {
            Type = type,
            Title = _staticTextLocalizer.LocalizeProblemTitle(titleKey, locale),
            Status = statusCode,
            Message = localizedError.Message,
            Instance = context.Request.Path,
            TraceId = _environment.IsDevelopment() ? context.TraceIdentifier : null,
            Errors = string.IsNullOrWhiteSpace(localizedError.FieldKey)
                ? null
                : new Dictionary<string, string[]>
                {
                    [localizedError.FieldKey] = [localizedError.Message]
                },
            Extensions = extensions
        };
    }

    private static string ResolveLocale(HttpContext context)
    {
        var header = context.Request.Headers.AcceptLanguage.ToString();
        if (string.IsNullOrWhiteSpace(header))
            return "en";

        return header.Split(',', ';')[0].Trim().ToLowerInvariant();
    }
}