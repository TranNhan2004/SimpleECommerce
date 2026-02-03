using SimpleECommerceBackend.Api.Middleware;

namespace SimpleECommerceBackend.Api.Extensions;

/// <summary>
///     Extension methods for registering exception handling
/// </summary>
public static class ExceptionHandlerExtension
{
    /// <summary>
    ///     Adds global exception handler middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}