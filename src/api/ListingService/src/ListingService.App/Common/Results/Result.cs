using ListingService.App.Common.Errors;
using static MongoDB.Driver.WriteConcern;

namespace ListingService.App.Common.Results;

public class Result<T>
{
    public T? Value { get; }
    public IError? Error { get; }

    public bool IsSuccess => Error is null;
    public bool IsFailure => !IsSuccess;

    private Result(T value)
    {
        Value = value;
        Error = null;
    }

    private Result(IError error)
    {
        Value = default;
        Error = error;
    }
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(IError error) => new(error);
}
