namespace Payments.App.Common.Errors;

public record Conflict(string Message) : IErrorResult { }