namespace Payments.App.Common.Errors;

public record InvalidPaymentOperation(string Message) : IErrorResult;