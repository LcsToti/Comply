using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentExceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid or unauthorized change to a payment status occurs.
/// </summary>
public class PaymentStatusChangeException(string message) : DomainException(message);