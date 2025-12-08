
namespace SalesService.App.Common;

public class Result<T>
{
    public T? Value { get; }
    public IErrorResult? Error { get; }

    public bool IsSuccess => Error is null;
    public bool IsFailure => !IsSuccess;

    private Result(T value)
    {
        Value = value;
        Error = null;
    }

    private Result(IErrorResult error)
    {
        Value = default;
        Error = error;
    }
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(IErrorResult error) => new(error);
}