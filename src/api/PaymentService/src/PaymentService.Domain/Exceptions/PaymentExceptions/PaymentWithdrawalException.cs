using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentExceptions;

/// <summary>
/// Represents an exception that is thrown when an issue occurs during the approval process
/// of a payment withdrawal. This indicates a business rule or domain-specific validation failure
/// related to payment withdrawal approvals.
/// </summary>
public class PaymentWithdrawalException(string message) : DomainException(message);