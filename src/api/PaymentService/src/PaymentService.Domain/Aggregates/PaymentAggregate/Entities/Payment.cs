using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;
using Payments.Domain.Aggregates.PaymentAggregate.VOs;
using Payments.Domain.Exceptions.PaymentExceptions;
using Payments.Domain.SeedWork;

namespace Payments.Domain.Aggregates.PaymentAggregate.Entities
{
    public class Payment : IAggregateRoot
    {
        public Guid Id { get; internal set; }
        public PaymentStatus Status { get; internal set; }
        public WithdrawalStatus WithdrawalStatus { get; internal set; }
        public string? PaymentMethod { get; internal set; }
        public Guid PayerId { get; internal set; }
        public Guid? SellerId { get; internal set; }
        public Amount Amount { get; internal set; }
        public Timestamps Timestamps { get; internal set; }
        public Guid SourceId { get; internal set; }
        public Gateway Gateway { get; internal set; }

        internal List<Refund> _refunds { get; } = [];
        public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();

        internal Payment(Gateway gateway, Amount amount, Guid? payerId, Guid? sellerId, PaymentStatus initialStatus, WithdrawalStatus initialWithdrawal)
        {
            Id = Guid.NewGuid();
            Status = initialStatus;
            WithdrawalStatus = initialWithdrawal;
            Gateway = gateway;
            Amount = amount;
            PayerId = payerId ?? Guid.Empty;
            SellerId = sellerId ?? Guid.Empty;
            Timestamps = Timestamps.Create(DateTime.UtcNow, null, DateTime.UtcNow, null);
        }

        /// <summary>
        /// Confirms the payment by updating its status to 'Succeeded', assigning the provided payment method ID,
        /// and setting the processed and updated timestamps. Throws an exception if the current payment
        /// status is not 'Pending'.
        /// </summary>
        /// <param name="confirmedPaymentMethodId">The ID of the payment method being confirmed.</param>
        /// <exception cref="PaymentStatusChangeException">
        /// Thrown when attempting to confirm a payment that does not have a status of 'Pending'.
        /// </exception>
        public void Confirm(string confirmedPaymentMethodId)
        {
            if (Status != PaymentStatus.Pending)
            {
                throw new PaymentStatusChangeException("Cannot confirm a payment that is not in the 'Pending' status.");
            }

            this.Status = PaymentStatus.Succeeded;
            this.PaymentMethod = confirmedPaymentMethodId;
            this.Timestamps = Timestamps with { ProcessedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        }

        /// <summary>
        /// Marks the payment as 'Failed' by updating its status and setting the processed and updated timestamps.
        /// Throws an exception if the current payment status is not 'Pending'.
        /// </summary>
        /// <exception cref="PaymentStatusChangeException">
        /// Thrown when attempting to fail a payment that does not have a 'Pending' status.
        /// </exception>
        public void Fail()
        {
            if (Status != PaymentStatus.Pending)
            {
                throw new PaymentStatusChangeException("Cannot fail a payment that is not in the 'Pending' status.");
            }
            
            Status = PaymentStatus.Failed;
            Timestamps = Timestamps with { ProcessedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        }

        /// <summary>
        /// Associates the payment with the specified source identifier.
        /// Useful for correlating the payment to its originating source.
        /// </summary>
        /// <param name="sourceId">The identifier of the source to be linked to the payment.</param>
        public void AddSource(Guid sourceId)
        {
            if (sourceId == Guid.Empty)
                throw new InvalidPaymentParamsException("The source cannot be null.");
            SourceId = sourceId;
        }

        /// <summary>
        /// Adds a payer to the payment by setting the payer ID. Throws an exception if the provided payer ID is invalid.
        /// </summary>
        /// <param name="payerId">The unique identifier of the payer to be associated with the payment.</param>
        /// <exception cref="InvalidPaymentParamsException">
        /// Thrown when an invalid payer ID is provided or when the current payer ID is not null.
        /// </exception>
        public void AddPayer(Guid payerId)
        {
            if (payerId == Guid.Empty)
                throw new InvalidPaymentParamsException("The payer cannot be null.");
            PayerId = payerId;
        }

        /// <summary>
        /// Associates a seller with this payment by setting the provided seller ID.
        /// Throws an exception if the current seller ID is invalid or not set.
        /// </summary>
        /// <param name="sellerId">The unique identifier of the seller to be associated with the payment.</param>
        /// <exception cref="InvalidPaymentParamsException">
        /// Thrown when the seller ID is not valid or is missing.
        /// </exception>
        public void AddSeller(Guid sellerId)
        {
            if (sellerId == Guid.Empty)
                throw new InvalidPaymentParamsException("The seller cannot be null.");
            SellerId = sellerId;
        }

        /// <summary>
        /// Ensures that the payment can be refunded for the specified amount by verifying its status and ensuring the total refundable amount has not been exceeded.
        /// </summary>
        /// <param name="amountToRefund">The amount of money intended to be refunded.</param>
        /// <exception cref="PaymentNotRefundableException">
        /// Thrown when the payment status does not allow refunds (statuses other than 'Succeeded' or 'PartiallyRefunded').
        /// </exception>
        /// <exception cref="PaymentRefundableAmountExceededException">
        /// Thrown when the total refunded amount exceeds the original payment amount.
        /// </exception>
        public bool EnsureCanBeRefunded(decimal amountToRefund)
        {
            if (Status != PaymentStatus.Succeeded && Status != PaymentStatus.PartiallyRefunded)
            {
                throw new PaymentNotRefundableException(
                    "Cannot refund a payment that is not in the 'Succeeded' or 'PartiallyRefunded' status.");
            }
            
            var totalRefunded = _refunds.Sum(r => r.RefundAmount);
            if (totalRefunded + amountToRefund > this.Amount.Total)
            {
                throw new PaymentRefundableAmountExceededException(
                    $"Cannot refund more than {this.Amount.Total - totalRefunded} because the payment is already refunded.");
            }
            
            return true;
        }

        /// <summary>
        /// Adds a new refund to the payment and updates the payment status based on the total refunded amount.
        /// If the total refunded amount matches the payment's total amount, the status is set to 'Refunded'.
        /// Otherwise, the status is set to 'PartiallyRefunded'.
        /// </summary>
        /// <param name="newRefund">The refund to be added, containing information such as the refund amount.</param>
        /// <returns>The refund that was added to the payment.</returns>
        public Refund AddRefund(Refund newRefund)
        {
            _refunds.Add(newRefund);

            var totalRefunded = _refunds.Sum(r => r.RefundAmount);
            Status = (totalRefunded >= this.Amount.Total) ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
            Timestamps = Timestamps with { UpdatedAt = DateTime.UtcNow };

            return newRefund;
        }

        /// <summary>
        /// Updates the withdrawal status of the payment to 'ApprovedToWithdraw'.
        /// Throws an exception if the current withdrawal status is not 'WaitingApproval'.
        /// </summary>
        /// <exception cref="PaymentStatusChangeException">
        /// Thrown when the payment cannot be marked as approved to withdraw because it is not in the 'WaitingApproval' status.
        /// </exception>
        public void MarkAsApprovedToWithdraw()
        {
            if (WithdrawalStatus != WithdrawalStatus.WaitingApproval)
                throw new PaymentStatusChangeException(
                    "Cannot mark a payment as approved to withdraw if it is not in the 'WaitingApproval' status.");
            if (Status != PaymentStatus.Succeeded)
                throw new PaymentStatusChangeException(
                    "Cannot mark a payment as approved to withdraw if it is not in the 'Succeeded' status.");
            
            WithdrawalStatus = WithdrawalStatus.ApprovedToWithdraw;
        }

        /// <summary>
        /// Marks the payment's withdrawal status as 'Withdrawing' if it is currently
        /// in the 'ApprovedToWithdraw' withdrawal status and the payment status is 'Succeeded'.
        /// Throws an exception if these conditions are not met.
        /// </summary>
        /// <exception cref="PaymentWithdrawalException">
        /// Thrown when the withdrawal status is not 'ApprovedToWithdraw' or the payment status is not 'Succeeded'.
        /// </exception>
        public void MarkAsWithdrawing()
        {
            if (WithdrawalStatus != WithdrawalStatus.ApprovedToWithdraw && WithdrawalStatus != WithdrawalStatus.Failed)
                throw new PaymentWithdrawalException("Cannot mark a payment as withdrawing if it is not in the 'ApprovedToWithdraw' status.");
            if (Status != PaymentStatus.Succeeded)
                throw new PaymentWithdrawalException("Cannot mark a payment as withdrawing if it is not in the 'Succeeded' status.");
            
            WithdrawalStatus = WithdrawalStatus.Withdrawing;
            Timestamps = Timestamps with { UpdatedAt = DateTime.UtcNow };
        }

        /// <summary>
        /// Marks the payment as withdrawn by updating its withdrawal status to 'AlreadyWithdrawn.'
        /// Throws an exception if the current withdrawal status is not 'Withdrawing' or the payment status is not 'Succeeded'.
        /// </summary>
        /// <exception cref="PaymentWithdrawalException">
        /// Thrown when the payment cannot be marked as withdrawn because its withdrawal status is not 'ApprovedToWithdraw.'
        /// </exception>
        public void MarkAsWithdrawn()
        {
            if (WithdrawalStatus != WithdrawalStatus.Withdrawing)
                throw new PaymentWithdrawalException("Cannot mark a payment as withdrawn if it is not in the 'Withdrawing' status.");
            if (Status != PaymentStatus.Succeeded)
                throw new PaymentWithdrawalException("Cannot mark a payment as withdrawn if it is not in the 'Succeeded' status.");
            
            WithdrawalStatus = WithdrawalStatus.AlreadyWithdrawn;
            Timestamps = Timestamps with { WithdrawnAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        }

        /// <summary>
        /// Marks the payment's withdrawal status as 'Failed'. Ensures that the payment's current withdrawal status
        /// is 'Withdrawing', the payment's status is 'Succeeded', and that the payment has not yet been
        /// fully withdrawn. Otherwise, an exception is thrown.
        /// </summary>
        /// <exception cref="PaymentWithdrawalException">
        /// Thrown if the withdrawal status is not 'ApprovedToWithdraw', the payment status is not 'Succeeded',
        /// or the withdrawal status is already set as 'AlreadyWithdrawn'.
        /// </exception>
        public void MarkAsFailedToWithdraw()
        {
            if (WithdrawalStatus != WithdrawalStatus.Withdrawing)
                throw new PaymentWithdrawalException("Cannot mark a payment as failed to withdraw if it is not in the 'Withdrawing' status.");
            if (Status != PaymentStatus.Succeeded)
                throw new PaymentWithdrawalException("Cannot mark a payment as failed to withdraw if it is not in the 'Succeeded' status.");
            if (WithdrawalStatus == WithdrawalStatus.AlreadyWithdrawn)
                throw new PaymentWithdrawalException("Cannot mark a payment as failed to withdraw if it is already withdrawn.");
            
            WithdrawalStatus = WithdrawalStatus.Failed;
            Timestamps = Timestamps with { UpdatedAt = DateTime.UtcNow };
        }
    }
}
