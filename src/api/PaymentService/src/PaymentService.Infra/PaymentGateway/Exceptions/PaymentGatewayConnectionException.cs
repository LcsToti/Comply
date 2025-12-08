namespace Payments.Infra.PaymentGateway.Exceptions;

/// <summary>
/// Represents an exception that is thrown when there is a failure in connecting
/// to a payment gateway during a transaction or communication process.
/// </summary>
public class PaymentGatewayConnectionException(string message) : Exception(message);