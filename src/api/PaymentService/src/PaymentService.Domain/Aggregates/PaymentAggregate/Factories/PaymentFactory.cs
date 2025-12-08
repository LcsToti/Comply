using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;
using Payments.Domain.Aggregates.PaymentAggregate.VOs;
using Payments.Domain.Exceptions;
using Payments.Domain.Exceptions.PaymentExceptions;

namespace Payments.Domain.Aggregates.PaymentAggregate.Factories;

public static class PaymentFactory
{
    /// <summary>
    /// Creates a new <see cref="Payment"/> instance with the provided gateway, amount, and payer identifier.
    /// </summary>
    /// <param name="gateway">The <see cref="Gateway"/> instance containing details about the payment gateway.</param>
    /// <param name="amount">The <see cref="Amount"/> object specifying the total value and currency of the payment.</param>
    /// <param name="payerId">The unique identifier of the payer who is associated with the payment.</param>
    /// <returns>A newly created <see cref="Payment"/> object with an initial status of <c>Pending</c>.</returns>
    /// <exception cref="RequiredValueObjectException">Thrown when the <paramref name="gateway"/> or <paramref name="amount"/> parameter is null.</exception>
    /// <exception cref="InvalidPaymentParamsException">Thrown when the <paramref name="amount"/> has an invalid total value or the <paramref name="payerId"/> is null.</exception>
    public static Payment CreatePayment(Gateway gateway, Amount amount)
    {
        if (gateway is null)
            throw new RequiredValueObjectException("The gateway cannot be null.");

        if (amount is null)
            throw new RequiredValueObjectException("The payment values cannot be null");

        if (amount.Total <= 0)
            throw new InvalidPaymentParamsException("Payment total value cannot be null.");

        var payment = new Payment(gateway, amount, Guid.Empty, Guid.Empty, PaymentStatus.Pending, WithdrawalStatus.WaitingApproval);

        return payment;
    }

    /// <summary>
    /// Restores a <see cref="Payment"/> entity from its persisted state, initializing it with the specified properties.
    /// </summary>
    /// <param name="id">The unique identifier of the payment.</param>
    /// <param name="status">The current status of the payment operation.</param>
    /// <param name="withdrawalStatus">The current withdrawal status of the payment.</param>
    /// <param name="paymentMethod">The payment method used, if any.</param>
    /// <param name="amount">The amount details associated with the payment.</param>
    /// <param name="gateway">Payment gateway information used for processing the payment.</param>
    /// <param name="payerId">The identifier of the payer associated with the payment.</param>
    /// <param name="sellerId">The identifier of the seller receiving the payment, if applicable.</param>
    /// <param name="sourceId">The identifier of the source entity related to the payment.</param>
    /// <param name="timestamps">Timestamps containing information about the creation and modification of the payment.</param>
    /// <param name="refunds">A collection of refunds associated with the payment, or null if no refunds exist.</param>
    /// <returns>A restored <see cref="Payment"/> instance populated with the provided state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any required parameter (id, gateway, amount, payerId, sourceId, or timestamps) is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the provided amount is invalid or its total value is less than or equal to zero.</exception>
    public static Payment LoadFromState(
        Guid id,
        PaymentStatus status,
        WithdrawalStatus withdrawalStatus,
        string? paymentMethod,
        Amount amount,
        Gateway gateway,
        Guid payerId,
        Guid? sellerId,
        Guid sourceId,
        Timestamps timestamps,
        IEnumerable<Refund>? refunds)
    {
        var payment = new Payment(gateway, amount, payerId, sellerId, status, withdrawalStatus)
        {
            Id = id,
            Status = status,
            WithdrawalStatus = withdrawalStatus,
            PaymentMethod = paymentMethod,
            SourceId = sourceId,
            Timestamps = timestamps
        };


        if (refunds != null)
        {
            foreach (var refund in refunds)
            {
                payment._refunds.Add(refund);
            }
        }
        return payment;
    }
}