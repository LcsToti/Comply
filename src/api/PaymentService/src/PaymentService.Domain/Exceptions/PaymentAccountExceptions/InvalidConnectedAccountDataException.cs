using Payments.Domain.Common;

namespace Payments.Domain.Exceptions.PaymentAccountExceptions;

/// <summary>
/// Represents an exception that is thrown when invalid data is provided for a connected account.
/// </summary>
public class InvalidConnectedAccountDataException(string message) : DomainException(message);