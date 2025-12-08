namespace SalesService.App.Common.Errors;

public record InvalidSaleOperation(string Message) : IErrorResult;