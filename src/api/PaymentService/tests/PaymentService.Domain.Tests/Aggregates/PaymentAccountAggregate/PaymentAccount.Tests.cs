using FluentAssertions;
using Payments.Domain.Aggregates.PaymentAccountAggregate;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Factories;
using Payments.Domain.Exceptions.PaymentAccountExceptions;

namespace Payments.Domain.Tests.Aggregates.PaymentAccountAggregate;

public class PaymentAccountTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private const string CustomerId = "cus_123";
    private const string ConnectedAccountId = "acct_123";
    private const string OnboardingLink = "https://onboard.link/123";

    private PaymentAccount CreateDefaultAccount()
    {
        return PaymentAccountFactory.Create(_userId, CustomerId, ConnectedAccountId, PaymentAccountStatus.Incomplete);
    }
    
    #region Factory Tests

    [Fact]
    public void Factory_Create_WithValidParams_ShouldCreateAccount()
    {
        // Act
        var account = CreateDefaultAccount();

        // Assert
        account.UserId.Should().Be(_userId);
        account.CustomerId.Should().Be(CustomerId);
        account.ConnectedAccountId.Should().Be(ConnectedAccountId);
    }

    [Fact]
    public void Factory_Create_WithEmptyUserId_ShouldThrowInvalidPaymentAccountParamsException()
    {
        // Act
        Action act = () => PaymentAccountFactory.Create(Guid.Empty, CustomerId, ConnectedAccountId, PaymentAccountStatus.Incomplete);
        
        // Assert
        act.Should().Throw<InvalidPaymentAccountParamsException>()
            .WithMessage("User ID cannot be null or empty.");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Factory_Create_WithInvalidCustomerId_ShouldThrowInvalidPaymentAccountParamsException(string invalidId)
    {
        // Act
        Action act = () => PaymentAccountFactory.Create(_userId, invalidId, ConnectedAccountId, PaymentAccountStatus.Incomplete);
        
        // Assert
        act.Should().Throw<InvalidPaymentAccountParamsException>()
            .WithMessage("Customer ID cannot be null or empty.");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Factory_Create_WithInvalidConnectedAccountId_ShouldThrowInvalidPaymentAccountParamsException(string invalidId)
    {
        // Act
        Action act = () =>
            PaymentAccountFactory.Create(_userId, CustomerId, invalidId, PaymentAccountStatus.Incomplete);
        
        // Assert
        act.Should().Throw<InvalidPaymentAccountParamsException>()
            .WithMessage("Connected Account ID cannot be null or empty.");
    }

    #endregion
}