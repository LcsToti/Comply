using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Exceptions.RefundExceptions;

namespace Payments.Domain.Aggregates.PaymentAggregate.Factories;

public static class RefundFactory
{
    /// <summary>
    /// Creates a new instance of the Refund class with the specified details.
    /// </summary>
    /// <param name="apiRefundId">The unique identifier of the refund from the external API.</param>
    /// <param name="amount">The amount to be refunded. Must be greater than zero.</param>
    /// <param name="reason">The reason for the refund. Cannot be null or empty.</param>
    /// <param name="status">The status of the refund. Cannot be null or empty.</param>
    /// <param name="createdAt">The date and time the refund was created.</param>
    /// <returns>A new Refund instance containing the specified details.</returns>
    /// <exception cref="InvalidRefundParamsException">Thrown when any required parameter (apiRefundId, reason, or status) is null or empty.</exception>
    /// <exception cref="InvalidRefundAmountException">Thrown when the amount is less than or equal to zero.</exception>
    public static Refund Create(string apiRefundId, decimal amount, string reason, string status, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(apiRefundId))
            throw new InvalidRefundParamsException("Refund ID cannot be null or empty.");

        if (amount <= 0)
            throw new InvalidRefundAmountException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidRefundParamsException("Reason cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(status))
            throw new InvalidRefundParamsException("Status cannot be null or empty.");

        return new Refund(apiRefundId, amount, reason, status, createdAt);
    }
}