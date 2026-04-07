namespace SimpleECommerceBackend.Domain.Exceptions;

public abstract class ExceptionBase : Exception
{
    public string ErrorCode { get; }
    public string? InternalMessage { get; }
    public IReadOnlyDictionary<string, object?>? Details { get; }

    protected ExceptionBase(string errorCode, string? internalMessage = null, IReadOnlyDictionary<string, object?>? details = null)
        : base(internalMessage ?? errorCode)
    {
        ErrorCode = errorCode;
        InternalMessage = internalMessage;
        Details = details;
    }
}
