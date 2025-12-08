using FluentAssertions;
using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;
using Payments.Domain.Aggregates.PaymentAggregate.Factories;
using Payments.Domain.Aggregates.PaymentAggregate.VOs;
using Payments.Domain.Exceptions.PaymentExceptions;

namespace Payments.Domain.Tests.Aggregates
{
    public class PaymentTests
    {
        private static Payment CreateDefaultPayment(decimal total = 100m,
            PaymentStatus status = PaymentStatus.Pending,
            WithdrawalStatus withdrawalStatus = WithdrawalStatus.WaitingApproval)
        {
            var amount = Amount.Create(total, "BRL");
            var gateway = Gateway.Create("MERCADO_PAGO", "gwy-abc", "chg-abc");
            var timestamps = Timestamps.Create(DateTime.UtcNow.AddMinutes(-10), null, DateTime.UtcNow.AddMinutes(-5), null);
            var payerId = Guid.NewGuid();
            var sellerId =Guid.NewGuid();
            var sourceId = Guid.NewGuid();

            var payment = PaymentFactory.LoadFromState(
                id: Guid.NewGuid(),
                status: PaymentStatus.Pending,
                withdrawalStatus: withdrawalStatus,
                paymentMethod: null,
                amount: amount,
                gateway: gateway,
                payerId: payerId,
                sellerId: sellerId,
                sourceId: sourceId,
                timestamps: timestamps,
                refunds: null
            );

            // Se o estado desejado for Succeeded ou um estado pós-Succeeded
            if (status == PaymentStatus.Succeeded || status == PaymentStatus.PartiallyRefunded || status == PaymentStatus.Refunded)
            {
                payment.Confirm("pm-123"); // Move para Succeeded
                payment.Status = status; // Força o estado final (ex: Refunded)
            }
            else
            {
                // Se o estado desejado for Pending ou Failed
                payment.Status = status;
            }
            
            // Garante que o WithdrawalStatus também seja o desejado
            payment.WithdrawalStatus = withdrawalStatus; 

            return payment;
        }

        #region Create/Add Tests (Novos)

        [Fact]
        public void AddPayer_WhenPayerIdIsEmpty_ShouldThrowInvalidPaymentParamsException()
        {
            // Arrange
            // Um pagamento recém-criado pela factory tem PayerId == Guid.Empty
            var payment = PaymentFactory.CreatePayment(
                gateway: Gateway.Create("MERCADO_PAGO", "gwy-abc", "chg-abc"),
                amount: Amount.Create(100, "BRL")
            );
            
            // Act
            Action act = () => payment.AddPayer(Guid.Empty);

            // Assert
            // Este teste falha com base na lógica de domínio fornecida: if (PayerId == Guid.Empty) throw...
            act.Should().Throw<InvalidPaymentParamsException>()
                .WithMessage("The payer cannot be null.");
        }

        [Fact]
        public void AddSeller_WhenSellerIdIsEmpty_ShouldThrowInvalidPaymentParamsException()
        {
            // Arrange
            // Um pagamento recém-criado pela factory tem SellerId == string.Empty
            var payment = PaymentFactory.CreatePayment(
                gateway: Gateway.Create("MERCADO_PAGO", "gwy-abc", "chg-abc"),
                amount: Amount.Create(100, "BRL")
            );

            // Act
            Action act = () => payment.AddSeller(Guid.Empty);

            // Assert
            act.Should().Throw<InvalidPaymentParamsException>()
                .WithMessage("The seller cannot be null.");
        }

        [Fact]
        public void AddSource_ShouldSetSourceId()
        {
            // Arrange
            var payment = CreateDefaultPayment();
            var sourceId = Guid.NewGuid();
            
            // Act
            payment.AddSource(sourceId);
            
            // Assert
            payment.SourceId.Should().Be(sourceId);
        }

        #endregion

        #region Confirm Tests

        [Fact]
        public void Confirm_WhenStatusIsPending_ShouldChangeStatusToSucceededAndSetPaymentMethod()
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Pending);
            const string paymentMethodId = "pm-xyz-123";

            // Act
            payment.Confirm(paymentMethodId);

            // Assert
            payment.Status.Should().Be(PaymentStatus.Succeeded);
            payment.PaymentMethod.Should().Be(paymentMethodId);
            payment.Timestamps.ProcessedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(PaymentStatus.Succeeded)]
        [InlineData(PaymentStatus.Failed)]
        [InlineData(PaymentStatus.Refunded)]
        [InlineData(PaymentStatus.PartiallyRefunded)]
        public void Confirm_WhenStatusIsNotPending_ShouldThrowPaymentStatusChangeException(PaymentStatus initialStatus)
        {
            // Arrange
            var payment = CreateDefaultPayment(status: initialStatus);
            Action act = () => payment.Confirm("pm-123");

            // Act & Assert
            act.Should().Throw<PaymentStatusChangeException>()
               .WithMessage("Cannot confirm a payment that is not in the 'Pending' status.");
        }

        #endregion

        #region Fail Tests

        [Fact]
        public void Fail_WhenStatusIsPending_ShouldChangeStatusToFailed()
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Pending);

            // Act
            payment.Fail();

            // Assert
            payment.Status.Should().Be(PaymentStatus.Failed);
            payment.Timestamps.ProcessedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(PaymentStatus.Succeeded)]
        [InlineData(PaymentStatus.Failed)]
        [InlineData(PaymentStatus.Refunded)]
        [InlineData(PaymentStatus.PartiallyRefunded)]
        public void Fail_WhenStatusIsNotPending_ShouldThrowPaymentStatusChangeException(PaymentStatus initialStatus)
        {
            // Arrange
            var payment = CreateDefaultPayment(status: initialStatus);

            // Act
            Action act = () => payment.Fail();

            // Assert
            act.Should().Throw<PaymentStatusChangeException>()
                .WithMessage("Cannot fail a payment that is not in the 'Pending' status.");
        }

        #endregion

        #region Refund Tests

        [Fact]
        public void EnsureCanBeRefunded_WhenAmountIsGreaterThanTotal_ShouldThrowPaymentRefundableAmountExceededException()
        {
            // Arrange
            var payment = CreateDefaultPayment(total: 100m, status: PaymentStatus.Succeeded);
            Action act = () => payment.EnsureCanBeRefunded(100.01m);

            // Act & Assert
            act.Should().Throw<PaymentRefundableAmountExceededException>()
               .WithMessage("Cannot refund more than 100 because the payment is already refunded."); // 100.00 - 0
        }
        
        [Fact]
        public void EnsureCanBeRefunded_WhenAmountExceedsRemainingPartial_ShouldThrowPaymentRefundableAmountExceededException()
        {
            // Arrange
            var payment = CreateDefaultPayment(total: 100m, status: PaymentStatus.PartiallyRefunded);
            var existingRefund = RefundFactory.Create("ref-1", 60m, "partial", "succeeded", DateTime.UtcNow);
            payment.AddRefund(existingRefund); // Total restante = 40m

            // Act
            Action act = () => payment.EnsureCanBeRefunded(40.01m); // Tenta estornar 40.01

            // Act & Assert
            act.Should().Throw<PaymentRefundableAmountExceededException>()
               .WithMessage("Cannot refund more than 40 because the payment is already refunded."); // 100.00 - 60.00
        }

        [Fact]
        public void AddRefund_ForPartialAmount_ShouldSetStatusToPartiallyRefunded()
        {
            // Arrange
            var payment = CreateDefaultPayment(total: 100m, status: PaymentStatus.Succeeded);
            var refund = RefundFactory.Create("gwy-abc", 40m, "estorno parcial", "succeeded", DateTime.UtcNow);

            // Act
            payment.EnsureCanBeRefunded(40m);
            payment.AddRefund(refund);

            // Assert
            payment.Refunds.Should().HaveCount(1);
            payment.Status.Should().Be(PaymentStatus.PartiallyRefunded);
        }

        [Fact]
        public void AddRefund_ForFullAmount_ShouldSetStatusToRefunded()
        {
            // Arrange
            var payment = CreateDefaultPayment(total: 100m, status: PaymentStatus.Succeeded);
            var refund = RefundFactory.Create("gwy-abc", 100m, "estorno total", "succeeded", DateTime.UtcNow);

            // Act
            payment.EnsureCanBeRefunded(100m);
            payment.AddRefund(refund);

            // Assert
            payment.Refunds.Should().HaveCount(1);
            payment.Refunds.First().RefundAmount.Should().Be(100m);
            payment.Status.Should().Be(PaymentStatus.Refunded);
        }

        [Fact]
        public void AddRefund_WithMultiplePartialRefundsToFull_ShouldSetStatusToRefunded()
        {
            // Arrange
            var payment = CreateDefaultPayment(total: 100m, status: PaymentStatus.Succeeded);
            var refund1 = RefundFactory.Create("gwy-abc", 50m, "estorno 1", "succeeded", DateTime.UtcNow);
            var refund2 = RefundFactory.Create("gwy-abc", 50m, "estorno 2", "succeeded", DateTime.UtcNow);

            // Act
            payment.EnsureCanBeRefunded(50m);
            payment.AddRefund(refund1);

            // Assert first refund
            payment.Status.Should().Be(PaymentStatus.PartiallyRefunded);

            // Act second refund
            payment.EnsureCanBeRefunded(50m);
            payment.AddRefund(refund2);

            // Assert second refund
            payment.Status.Should().Be(PaymentStatus.Refunded);
            payment.Refunds.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(PaymentStatus.Pending)]
        [InlineData(PaymentStatus.Failed)]
        [InlineData(PaymentStatus.Refunded)]
        public void EnsureCanBeRefunded_WhenPaymentStatusIsNotRefundable_ShouldThrowPaymentNotRefundableException(PaymentStatus status)
        {
            // Arrange
            var payment = CreateDefaultPayment(status: status);
            Action act = () => payment.EnsureCanBeRefunded(50m);

            // Act & Assert
            act.Should().Throw<PaymentNotRefundableException>()
                .WithMessage("Cannot refund a payment that is not in the 'Succeeded' or 'PartiallyRefunded' status.");
        }

        #endregion

        #region Withdrawal Tests (Atualizados e Novos)

        [Fact]
        public void MarkAsApprovedToWithdraw_WhenWaitingApprovalAndSucceeded_ShouldSucceed()
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Succeeded, withdrawalStatus: WithdrawalStatus.WaitingApproval);

            // Act
            payment.MarkAsApprovedToWithdraw();

            // Assert
            payment.WithdrawalStatus.Should().Be(WithdrawalStatus.ApprovedToWithdraw);
        }

        [Theory]
        [InlineData(WithdrawalStatus.ApprovedToWithdraw)]
        [InlineData(WithdrawalStatus.Withdrawing)]
        [InlineData(WithdrawalStatus.AlreadyWithdrawn)]
        [InlineData(WithdrawalStatus.Failed)]
        public void MarkAsApprovedToWithdraw_WhenWithdrawalStatusIsNotWaitingApproval_ShouldThrowPaymentStatusChangeException(WithdrawalStatus status)
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Succeeded, withdrawalStatus: status);
            Action act = () => payment.MarkAsApprovedToWithdraw();

            // Act & Assert
            act.Should().Throw<PaymentStatusChangeException>()
                .WithMessage("Cannot mark a payment as approved to withdraw if it is not in the 'WaitingApproval' status.");
        }
        
        [Theory]
        [InlineData(PaymentStatus.Pending)]
        [InlineData(PaymentStatus.Failed)]
        [InlineData(PaymentStatus.Refunded)]
        [InlineData(PaymentStatus.PartiallyRefunded)]
        public void MarkAsApprovedToWithdraw_WhenPaymentStatusIsNotSucceeded_ShouldThrowPaymentStatusChangeException(PaymentStatus status)
        {
            // Arrange
            var payment = CreateDefaultPayment(status: status, withdrawalStatus: WithdrawalStatus.WaitingApproval);
            Action act = () => payment.MarkAsApprovedToWithdraw();

            // Act & Assert
            act.Should().Throw<PaymentStatusChangeException>()
                .WithMessage("Cannot mark a payment as approved to withdraw if it is not in the 'Succeeded' status.");
        }

        [Fact]
        public void MarkAsWithdrawing_WhenApprovedToWithdrawAndSucceeded_ShouldSucceed()
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Succeeded, withdrawalStatus: WithdrawalStatus.ApprovedToWithdraw);
            
            // Act
            payment.MarkAsWithdrawing();
            
            // Assert
            payment.WithdrawalStatus.Should().Be(WithdrawalStatus.Withdrawing);
        }
        
        [Theory]
        [InlineData(WithdrawalStatus.WaitingApproval)]
        [InlineData(WithdrawalStatus.Withdrawing)]
        [InlineData(WithdrawalStatus.AlreadyWithdrawn)]
        public void MarkAsWithdrawing_WhenWithdrawalStatusIsNotApproved_ShouldThrowPaymentWithdrawalException(WithdrawalStatus status)
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Succeeded, withdrawalStatus: status);
            Action act = () => payment.MarkAsWithdrawing();
            
            // Act & Assert
            act.Should().Throw<PaymentWithdrawalException>()
                .WithMessage("Cannot mark a payment as withdrawing if it is not in the 'ApprovedToWithdraw' status.");
        }
        
        [Fact]
        public void MarkAsWithdrawn_WhenWithdrawingAndSucceeded_ShouldSucceed()
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Succeeded, withdrawalStatus: WithdrawalStatus.Withdrawing);

            // Act
            payment.MarkAsWithdrawn();

            // Assert
            payment.WithdrawalStatus.Should().Be(WithdrawalStatus.AlreadyWithdrawn);
            payment.Timestamps.WithdrawnAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(WithdrawalStatus.WaitingApproval)]
        [InlineData(WithdrawalStatus.ApprovedToWithdraw)]
        [InlineData(WithdrawalStatus.AlreadyWithdrawn)]
        [InlineData(WithdrawalStatus.Failed)]
        public void MarkAsWithdrawn_WhenWithdrawalStatusIsNotWithdrawing_ShouldThrowPaymentWithdrawalException(WithdrawalStatus status)
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Succeeded, withdrawalStatus: status);
            Action act = () => payment.MarkAsWithdrawn();

            // Act & Assert
            act.Should().Throw<PaymentWithdrawalException>()
                .WithMessage("Cannot mark a payment as withdrawn if it is not in the 'Withdrawing' status.");
        }
        
        [Fact]
        public void MarkAsFailedToWithdraw_WhenWithdrawingAndSucceeded_ShouldSucceed()
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Succeeded, withdrawalStatus: WithdrawalStatus.Withdrawing);
            
            // Act
            payment.MarkAsFailedToWithdraw();
            
            // Assert
            payment.WithdrawalStatus.Should().Be(WithdrawalStatus.Failed);
        }
        
        [Fact]
        public void MarkAsFailedToWithdraw_WhenStatusIsNotSucceeded_ShouldThrowPaymentWithdrawalException()
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Failed, withdrawalStatus: WithdrawalStatus.Withdrawing);
            
            // Act
            Action act = () => payment.MarkAsFailedToWithdraw();
            
            // Assert
            act.Should().Throw<PaymentWithdrawalException>()
                .WithMessage("Cannot mark a payment as failed to withdraw if it is not in the 'Succeeded' status.");
        }
        
        [Fact]
        public void MarkAsFailedToWithdraw_WhenAlreadyWithdrawn_ShouldThrowPaymentWithdrawalException()
        {
            // Arrange
            var payment = CreateDefaultPayment(status: PaymentStatus.Succeeded, withdrawalStatus: WithdrawalStatus.AlreadyWithdrawn);
            
            // Act
            Action act = () => payment.MarkAsFailedToWithdraw();
            
            // Assert
            // A primeira verificação (WithdrawalStatus != Withdrawing) irá falhar primeiro.
            act.Should().Throw<PaymentWithdrawalException>()
                .WithMessage("Cannot mark a payment as failed to withdraw if it is not in the 'Withdrawing' status.");
        }

        #endregion
    }
}