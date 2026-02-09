using SimpleECommerceBackend.Api.DTOs.Errors;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Api.Middleware;

/// <summary>
///     Global exception handler middleware.
///     Catches all unhandled exceptions and map to appropriate HTTP responses.
/// </summary>
public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment
    )
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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
        // Log exception với appropriate level
        LogException(exception);

        // Map exception to HTTP response
        var (statusCode, errorResponse) = MapExceptionToResponse(context, exception);

        // Set response
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private void LogException(Exception exception)
    {
        switch (exception)
        {
            case DomainException:
            case NotFoundException:
            case ConflictException:
                // Expected exceptions - log as warning
                _logger.LogWarning(exception,
                    "Domain exception occurred: {ExceptionType} - {Message}",
                    exception.GetType().Name,
                    exception.Message);
                break;

            case UnauthorizedException:
            case ForbiddenException:
                // Security-related - log as warning with details
                _logger.LogWarning(exception,
                    "Security exception occurred: {ExceptionType} - {Message}",
                    exception.GetType().Name,
                    exception.Message);
                break;

            default:
                // Unexpected exceptions - log as error
                _logger.LogError(exception,
                    "Unhandled exception occurred: {ExceptionType} - {Message}",
                    exception.GetType().Name,
                    exception.Message);
                break;
        }
    }

    private (int StatusCode, ErrorResponse Response) MapExceptionToResponse(HttpContext context, Exception exception)
    {
        return exception switch
        {
            DomainException domainException => (
                StatusCodes.Status422UnprocessableEntity,
                new ErrorResponse
                {
                    Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                    Title = "One or more validation errors occurred",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Detail = domainException.Message,
                    Instance = context.Request.Path,
                    TraceId = context.TraceIdentifier
                }
            ),

            // BusinessException businessEx => (
            //     StatusCodes.Status400BadRequest,
            //     new ErrorResponse
            //     {
            //         Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            //         Title = "Business rule violation",
            //         Status = StatusCodes.Status400BadRequest,
            //         Detail = businessEx.Message,
            //         Instance = context.Request.Path,
            //         TraceId = context.TraceIdentifier,
            //         Extensions = businessEx.Code is not null
            //             ? new Dictionary<string, object> { ["code"] = businessEx.Code }
            //             : null
            //     }
            // ),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new ErrorResponse
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Resource not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = notFoundEx.Message,
                    Instance = context.Request.Path,
                    TraceId = context.TraceIdentifier,
                    Extensions = new Dictionary<string, object>
                    {
                        ["entityName"] = notFoundEx.EntityName,
                        ["entityId"] = notFoundEx.EntityId
                    }
                }
            ),

            ConflictException conflictEx => (
                StatusCodes.Status409Conflict,
                new ErrorResponse
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    Title = "Resource conflict",
                    Status = StatusCodes.Status409Conflict,
                    Detail = conflictEx.Message,
                    Instance = context.Request.Path,
                    TraceId = context.TraceIdentifier,
                    Extensions = conflictEx.EntityName is not null && conflictEx.ConflictingField is not null
                        ? new Dictionary<string, object>
                        {
                            ["entityName"] = conflictEx.EntityName,
                            ["conflictingField"] = conflictEx.ConflictingField,
                            ["conflictingValue"] = conflictEx.ConflictingValue ?? string.Empty
                        }
                        : null
                }
            ),

            UnauthorizedException unauthorizedEx => (
                StatusCodes.Status401Unauthorized,
                new ErrorResponse
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = unauthorizedEx.Message,
                    Instance = context.Request.Path,
                    TraceId = context.TraceIdentifier
                }
            ),

            ForbiddenException forbiddenEx => (
                StatusCodes.Status403Forbidden,
                new ErrorResponse
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = "Forbidden",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = forbiddenEx.Message,
                    Instance = context.Request.Path,
                    TraceId = context.TraceIdentifier,
                    Extensions = forbiddenEx.Resource is not null
                        ? new Dictionary<string, object>
                        {
                            ["resource"] = forbiddenEx.Resource,
                            ["action"] = forbiddenEx.Action ?? string.Empty
                        }
                        : null
                }
            ),

            // Catch-all for unexpected exceptions
            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "An error occurred while processing your request",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = _environment.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred. Please try again later.",
                    Instance = context.Request.Path,
                    TraceId = context.TraceIdentifier,
                    // Include stack trace only in development
                    Extensions = _environment.IsDevelopment()
                        ? new Dictionary<string, object>
                        {
                            ["stackTrace"] = exception.StackTrace ?? string.Empty,
                            ["exceptionType"] = exception.GetType().FullName ?? string.Empty
                        }
                        : null
                }
            )
        };
    }
}