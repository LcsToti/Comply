using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.RefundExceptions;

/// <summary>
/// Represents an exception that is thrown when invalid parameters are provided for a refund operation.
/// </summary>
public class InvalidRefundParamsException(string message) : DomainException(message);