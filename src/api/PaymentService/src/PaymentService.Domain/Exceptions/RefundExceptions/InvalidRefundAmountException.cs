using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.RefundExceptions;

/// <summary>
/// Exception thrown when a refund amount is invalid.
/// </summary>
public class InvalidRefundAmountException(string message) : DomainException(message);