namespace SalesService.App.Common.Errors;

public record Forbidden(string Message) : IErrorResult { }