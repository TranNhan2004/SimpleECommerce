namespace SimpleECommerceBackend.Api.DTOs.Errors;

/// <summary>
///     Standard error response model following RFC 7807 Problem Details
/// </summary>
public sealed class ErrorResponse
{
    /// <summary>
    ///     URI reference that identifies the problem type
    /// </summary>
    public string Type { get; set; } = "about:blank";

    /// <summary>
    ///     Short, human-readable summary
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    ///     HTTP status code
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    ///     Human-readable explanation specific to this occurrence
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    ///     URI reference that identifies the specific occurrence
    /// </summary>
    public string? Instance { get; set; }

    /// <summary>
    ///     Trace identifier for debugging
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    ///     Validation errors (for 422 responses)
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    ///     Additional metadata
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }
}