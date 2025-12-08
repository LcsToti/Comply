namespace Payments.App.Common.Errors;

public record Forbidden(string Message) : IErrorResult { }