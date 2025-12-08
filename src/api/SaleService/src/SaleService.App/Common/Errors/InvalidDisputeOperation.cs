namespace SalesService.App.Common.Errors;

public record InvalidDisputeOperation(string Message) : IErrorResult;