using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentAccountExceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid customer ID is encountered.
/// </summary>
public class InvalidCustomerIdException(string message) : DomainException(message);