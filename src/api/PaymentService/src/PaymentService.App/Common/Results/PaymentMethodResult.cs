namespace Payments.App.Common.Results;

public record PaymentMethodResult(
    string Id, 
    string Type, 
    string? Last4,
    string? Brand);