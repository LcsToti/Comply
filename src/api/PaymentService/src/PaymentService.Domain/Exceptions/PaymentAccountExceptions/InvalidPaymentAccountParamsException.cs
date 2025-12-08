using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentAccountExceptions;

/// <summary>
/// Represents an exception that is thrown when the parameters provided for a payment account are invalid.
/// </summary>
public class InvalidPaymentAccountParamsException(string message) : DomainException(message);