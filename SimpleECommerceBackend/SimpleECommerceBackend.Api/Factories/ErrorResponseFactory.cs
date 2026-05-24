using SimpleECommerceBackend.Api.Dtos.Common.Errors;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Api.Factories;

public static class ErrorResponseFactory
{
    public static ErrorResponse CreateFromException(
        HttpContext context,
        ExceptionBase exception,
        int statusCode,
        string titleKey,
        string type,
        IHostEnvironment environment,
        IStaticTextLocalizer staticTextLocalizer
    )
    {
        return Create(
            context,
            statusCode,
            titleKey,
            type,
            exception.ErrorCode,
            environment,
            staticTextLocalizer,
            exception.Details,
            exception.InternalMessage
        );
    }

    public static ErrorResponse Create(
        HttpContext context,
        int statusCode,
        string titleKey,
        string type,
        string errorCode,
        IHostEnvironment environment,
        IStaticTextLocalizer staticTextLocalizer,
        IReadOnlyDictionary<string, object?>? details = null,
        string? internalMessage = null
    )
    {
        var locale = ResolveLocale(context);
        var localizedError = staticTextLocalizer.LocalizeError(errorCode, details, locale);
        var localizedTitle = staticTextLocalizer.LocalizeProblemTitle(titleKey, locale);
        var extensions = new Dictionary<string, object>
        {
            ["errorCode"] = errorCode,
            ["locale"] = locale
        };

        if (details is not null)
            extensions["details"] = details;

        if (!string.IsNullOrWhiteSpace(localizedError.FieldKey))
            extensions["field"] = localizedError.FieldKey;

        if (!string.IsNullOrWhiteSpace(localizedError.FieldDisplayName))
            extensions["fieldDisplayName"] = localizedError.FieldDisplayName;

        if (environment.IsDevelopment() && !string.IsNullOrWhiteSpace(internalMessage))
            extensions["internalMessage"] = internalMessage;

        return new ErrorResponse
        {
            Type = type,
            Title = localizedTitle,
            Status = statusCode,
            Message = localizedError.Message,
            Instance = context.Request.Path,
            TraceId = environment.IsDevelopment() ? context.TraceIdentifier : null,
            Errors = string.IsNullOrWhiteSpace(localizedError.FieldKey)
                ? null
                : new Dictionary<string, string[]>
                {
                    [localizedError.FieldKey] = [localizedError.Message]
                },
            Extensions = extensions
        };
    }

    public static ErrorResponse CreateUnexpected(
        HttpContext context,
        Exception exception,
        IHostEnvironment environment,
        IStaticTextLocalizer staticTextLocalizer
    )
    {
        var locale = ResolveLocale(context);

        return new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = staticTextLocalizer.LocalizeProblemTitle("Problem.Unexpected", locale),
            Status = StatusCodes.Status500InternalServerError,
            Message = exception.Message,
            Instance = context.Request.Path,
            TraceId = environment.IsDevelopment() ? context.TraceIdentifier : null,
            Extensions = new Dictionary<string, object>
            {
                ["locale"] = locale
            }
        };
    }

    public static string ResolveLocale(HttpContext context)
    {
        var header = context.Request.Headers.AcceptLanguage.ToString();
        if (string.IsNullOrWhiteSpace(header))
            return "en";

        return header.Split(',', ';')[0].Trim().ToLowerInvariant();
    }
}
