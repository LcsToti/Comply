using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentExceptions;

/// <summary>
/// Represents an exception that is thrown when a payment refund amount exceeds the allowed refundable amount.
/// </summary>
public class PaymentRefundableAmountExceededException(string message) : DomainException(message);