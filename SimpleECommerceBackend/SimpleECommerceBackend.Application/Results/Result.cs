namespace SimpleECommerceBackend.Application.Common;

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public Error? Error { get; }

    public static Result Ok()
    {
        return new Result(true, null);
    }

    public static Result Fail(Error error)
    {
        return new Result(false, error);
    }
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, Error? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Ok(T value)
    {
        return new Result<T>(true, value, null);
    }

    public new static Result<T> Fail(Error error)
    {
        return new Result<T>(false, default, error);
    }
}