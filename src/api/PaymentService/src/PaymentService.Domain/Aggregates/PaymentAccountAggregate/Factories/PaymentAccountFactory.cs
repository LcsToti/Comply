using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Exceptions.PaymentAccountExceptions;

namespace Payments.Domain.Aggregates.PaymentAccountAggregate.Factories;

public static class PaymentAccountFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref="PaymentAccount"/> class using the provided parameters.
    /// </summary>
    /// <param name="userId">The unique identifier of the user associated with the customer.</param>
    /// <param name="customerId">The unique identifier of the customer to be created.</param>
    /// <param name="connectedAccountId">The identifier of the connected account for the customer.</param>
    /// <param name="accountStatus"></param>
    /// <returns>A new instance of the <see cref="PaymentAccount"/> class with the specified details.</returns>
    /// <exception cref="InvalidPaymentAccountParamsException">
    /// Thrown when any of the provided parameters are null, empty, or contain only whitespace.
    /// </exception>
    public static PaymentAccount Create(Guid userId, string customerId, string connectedAccountId, PaymentAccountStatus accountStatus)
    {
        if (userId == Guid.Empty)
            throw new InvalidPaymentAccountParamsException("User ID cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(customerId))
            throw new InvalidPaymentAccountParamsException("Customer ID cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(connectedAccountId))
            throw new InvalidPaymentAccountParamsException("Connected Account ID cannot be null or empty.");
        
        return new PaymentAccount(userId, customerId, connectedAccountId, accountStatus);
    }
}