using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentMethodExceptions;

/// <summary>
/// This exception is thrown when invalid parameters are provided for a payment method.
/// </summary>
public class InvalidPaymentMethodParamsException(string message) : DomainException(message);