namespace Payments.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a required value object is missing or invalid.
/// </summary>
public class RequiredValueObjectException(string message) : Exception(message);