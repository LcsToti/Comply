namespace Payments.App.Common.Errors;

public record InvalidRefundOperation(string Message) : IErrorResult;