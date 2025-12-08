using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentExceptions;

/// <summary>
/// Represents an exception that is thrown when an attempt is made to refund
/// a payment that is not eligible for a refund.
/// </summary>
public class PaymentNotRefundableException(string message) : DomainException(message);