namespace NotificationService.App.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public ResultStatus Status { get; }
        public string Error { get; } = string.Empty;

        protected Result(bool isSuccess, ResultStatus status, string error)
        {
            IsSuccess = isSuccess;
            Status = status;
            Error = error;
        }
        public static Result Success() => new(true, ResultStatus.Success, string.Empty);
        public static Result Fail(string message, ResultStatus status = ResultStatus.Error) => new(false, status, message);
    }
    public class Result<T> : Result
    {
        public T Value { get; } = default!;

        protected Result(T value, bool isSuccess, ResultStatus status, string error)
            : base(isSuccess, status, error)
        {
            Value = value;
        }
        public static Result<T> Success(T value) => new(value, true, ResultStatus.Success, string.Empty);
        public new static Result<T> Fail(string message, ResultStatus status = ResultStatus.Error) => new(default!, false, status, message);
    }
}
