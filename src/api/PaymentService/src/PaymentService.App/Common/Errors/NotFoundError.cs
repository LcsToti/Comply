namespace Payments.App.Common.Errors;

public record NotFoundError(object Id, string Details) : IErrorResult
{
    public string Message => $"The resource with ID '{Id}' wasn't found.. {Details}";
}