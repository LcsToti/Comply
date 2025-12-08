using FluentAssertions;
using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;
using Payments.Domain.Aggregates.PaymentAggregate.Factories;
using Payments.Domain.Aggregates.PaymentAggregate.VOs;
using Payments.Domain.Exceptions;
using Payments.Domain.Exceptions.PaymentExceptions;

namespace Payments.Domain.Tests.Aggregates.PaymentAggregate.Factories;

public class PaymentFactoryTests
{
    private readonly Gateway _gateway = Gateway.Create("MERCADO_PAGO", "gwy-123", "chg-123");
    private readonly Amount _amount = Amount.Create(100m, "BRL");

    [Fact]
    public void CreatePayment_WithValidParams_ShouldCreatePendingPayment()
    {
        // Act
        var payment = PaymentFactory.CreatePayment(_gateway, _amount);

        // Assert
        payment.Should().NotBeNull();
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.WithdrawalStatus.Should().Be(WithdrawalStatus.WaitingApproval);
        payment.Gateway.Should().Be(_gateway);
        payment.Amount.Should().Be(_amount);
        payment.PayerId.Should().Be(Guid.Empty);
        payment.SellerId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CreatePayment_WithNullGateway_ShouldThrowRequiredValueObjectException()
    {
        // Act
        Action act = () => PaymentFactory.CreatePayment(null, _amount);

        // Assert
        act.Should().Throw<RequiredValueObjectException>()
            .WithMessage("The gateway cannot be null.");
    }

    [Fact]
    public void CreatePayment_WithNullAmount_ShouldThrowRequiredValueObjectException()
    {
        // Act
        Action act = () => PaymentFactory.CreatePayment(_gateway, null);

        // Assert
        act.Should().Throw<RequiredValueObjectException>()
            .WithMessage("The payment values cannot be null");
    }

    [Fact]
    public void CreatePayment_WithZeroTotalAmount_ShouldThrowInvalidPaymentParamsException()
    {
        // Arrange
        var zeroAmount = Amount.Create(0, "BRL");
        
        // Act
        Action act = () => PaymentFactory.CreatePayment(_gateway, zeroAmount);

        // Assert
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("Payment total value cannot be null.");
    }
    
    [Fact]
    public void LoadFromState_WithValidState_ShouldRestorePayment()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = PaymentStatus.Succeeded;
        var withdrawalStatus = WithdrawalStatus.ApprovedToWithdraw;
        var paymentMethod = "pm_123";
        var payerId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var timestamps = Timestamps.Create(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow.AddMinutes(-2), DateTime.UtcNow, null);

        // Act
        var payment = PaymentFactory.LoadFromState(id, status, withdrawalStatus, paymentMethod, _amount, _gateway,
            payerId, sellerId, sourceId, timestamps, null);

        // Assert
        payment.Id.Should().Be(id);
        payment.Status.Should().Be(status);
        payment.WithdrawalStatus.Should().Be(withdrawalStatus);
        payment.PaymentMethod.Should().Be(paymentMethod);
        payment.Amount.Should().Be(_amount);
        payment.Gateway.Should().Be(_gateway);
        payment.PayerId.Should().Be(payerId);
        payment.SellerId.Should().Be(sellerId);
        payment.SourceId.Should().Be(sourceId);
        payment.Timestamps.Should().Be(timestamps);
        payment.Refunds.Should().BeEmpty();
    }
    
    [Fact]
    public void LoadFromState_WithRefunds_ShouldRestorePaymentAndRefunds()
    {
        // Arrange
        var refund1 = RefundFactory.Create("ref_1", 50m, "reason 1", "succeeded", DateTime.UtcNow);
        var refunds = new List<Refund> { refund1 };
        
        // Act
        var payment = PaymentFactory.LoadFromState(Guid.NewGuid(), PaymentStatus.PartiallyRefunded, 
            WithdrawalStatus.WaitingApproval, "pm_123", _amount, _gateway,
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 
            Timestamps.Create(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow.AddMinutes(-2), DateTime.UtcNow, null), 
            refunds);

        // Assert
        payment.Refunds.Should().HaveCount(1);
        payment.Refunds.First().Should().Be(refund1);
        // O status será recalculado pelo AddRefund dentro do LoadFromState
        payment.Status.Should().Be(PaymentStatus.PartiallyRefunded);
    }
}