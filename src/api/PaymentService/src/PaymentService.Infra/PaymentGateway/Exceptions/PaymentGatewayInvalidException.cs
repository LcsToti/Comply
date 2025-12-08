namespace Payments.Infra.PaymentGateway.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a request made to the payment gateway is invalid.
/// This could occur due to malformed request parameters, missing required fields, or any other
/// client-side issue that violates the API contract of the payment gateway.
/// </summary>
public class PaymentGatewayInvalidException(string message) : Exception(message);