namespace Payments.Infra.PaymentGateway.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a payment processing operation fails.
/// </summary>
public class PaymentProcessingFailedException(string message) : Exception(message);