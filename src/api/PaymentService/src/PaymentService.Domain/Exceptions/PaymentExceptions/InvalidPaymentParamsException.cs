using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentExceptions;

/// <summary>
/// Represents an exception that is thrown when invalid parameters are provided for a payment operation.
/// </summary>
public class InvalidPaymentParamsException(string message) : DomainException(message);