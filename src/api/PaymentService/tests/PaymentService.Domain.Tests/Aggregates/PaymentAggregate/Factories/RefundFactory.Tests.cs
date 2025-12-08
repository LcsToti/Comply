using FluentAssertions;
using Payments.Domain.Aggregates.PaymentAggregate.Factories;
using Payments.Domain.Exceptions.RefundExceptions;

namespace Payments.Domain.Tests.Aggregates.PaymentAggregate.Factories;

public class RefundFactoryTests
{
    [Fact]
    public void Create_WithValidParams_ShouldCreateRefund()
    {
        // Arrange
        const string apiRefundId = "ref_123";
        const decimal amount = 100m;
        const string reason = "test reason";
        const string status = "succeeded";
        var createdAt = DateTime.UtcNow;

        // Act
        var refund = RefundFactory.Create(apiRefundId, amount, reason, status, createdAt);

        // Assert
        refund.Should().NotBeNull();
        refund.Id.Should().NotBeEmpty();
        refund.ApiRefundId.Should().Be(apiRefundId);
        refund.RefundAmount.Should().Be(amount);
        refund.Reason.Should().Be(reason);
        refund.RefundStatus.Should().Be(status);
        refund.CreatedAt.Should().Be(createdAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidApiRefundId_ShouldThrowInvalidRefundParamsException(string invalidId)
    {
        // Act
        Action act = () => RefundFactory.Create(invalidId, 100m, "reason", "status", DateTime.UtcNow);
        
        // Assert
        act.Should().Throw<InvalidRefundParamsException>()
            .WithMessage("Refund ID cannot be null or empty.");
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Create_WithInvalidAmount_ShouldThrowInvalidRefundAmountException(decimal invalidAmount)
    {
        // Act
        Action act = () => RefundFactory.Create("ref_123", invalidAmount, "reason", "status", DateTime.UtcNow);
        
        // Assert
        act.Should().Throw<InvalidRefundAmountException>()
            .WithMessage("Amount must be greater than zero.");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidReason_ShouldThrowInvalidRefundParamsException(string invalidReason)
    {
        // Act
        Action act = () => RefundFactory.Create("ref_123", 100m, invalidReason, "status", DateTime.UtcNow);
        
        // Assert
        act.Should().Throw<InvalidRefundParamsException>()
            .WithMessage("Reason cannot be null or empty.");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidStatus_ShouldThrowInvalidRefundParamsException(string invalidStatus)
    {
        // Act
        Action act = () => RefundFactory.Create("ref_123", 100m, "reason", invalidStatus, DateTime.UtcNow);
        
        // Assert
        act.Should().Throw<InvalidRefundParamsException>()
            .WithMessage("Status cannot be null or empty.");
    }
}