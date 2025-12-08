using FluentAssertions;
using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.AuctionAggregate.Enums;
using ListingService.Domain.Common;
using ListingService.Domain.Exceptions;

namespace ListingService.Domain.Tests.AuctionAggregate.Entities;

public class BidTests
{
    private readonly DateTime _fixedNow = new(2025, 10, 20, 10, 0, 0, DateTimeKind.Utc);
    [Fact]
    public void Create_WithValidParameters_ShouldCreateWinningBid()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auctionId = Guid.NewGuid();
        var bidderId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var value = 100m;
        var biddedAt = utcNow;

        // Act
        var bid = Bid.Create(auctionId, bidderId, paymentId, value, biddedAt);

        // Assert
        bid.Should().NotBeNull();
        bid.AuctionId.Should().Be(auctionId);
        bid.BidderId.Should().Be(bidderId);
        bid.PaymentId.Should().Be(paymentId);
        bid.Value.Should().Be(value);
        bid.Status.Should().Be(BidStatus.Winning);
        bid.BiddedAt.Should().Be(biddedAt);
        bid.OutbiddedAt.Should().BeNull();
        bid.WonAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithValueBelowMinimum_ShouldThrowInvalidBidException()
    {
        // Arrange
        var invalidValue = DomainConstants.MinTransactionValue - 0.01m;
        var utcNow = _fixedNow;

        // Act
        Action act = () => Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), invalidValue, utcNow);

        // Assert
        act.Should().Throw<InvalidBidException>()
            .WithMessage($"Bid value must be at least {DomainConstants.MinTransactionValue:C}.");
    }

    [Fact]
    public void Create_WithValueAboveMaximum_ShouldThrowInvalidBidException()
    {
        // Arrange
        var invalidValue = DomainConstants.MaxTransactionValue + 0.01m;
        var utcNow = _fixedNow;

        // Act
        Action act = () => Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), invalidValue, utcNow);

        // Assert
        act.Should().Throw<InvalidBidException>()
            .WithMessage($"Bid value should not exceed {DomainConstants.MaxTransactionValue:C}.");
    }

    [Fact]
    public void MarkAsOutbid_WhenBidIsWinning_ShouldTransitionToOutbid()
    {
        // Arrange
        var utcNow = _fixedNow;
        var bid = Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m, utcNow);
        var outbidTime = utcNow.AddSeconds(10);

        // Act
        bid.MarkAsOutbid(outbidTime);

        // Assert
        bid.Status.Should().Be(BidStatus.Outbid);
        bid.OutbiddedAt.Should().Be(outbidTime);
    }

    [Theory]
    [InlineData(BidStatus.Outbid)]
    [InlineData(BidStatus.Winner)]
    public void MarkAsOutbid_WhenBidIsNotWinning_ShouldThrowInvalidBidException(BidStatus initialStatus)
    {
        // Arrange
        var utcNow = _fixedNow;
        var bid = Bid.Restore(
            id: Guid.NewGuid(),
            auctionId: Guid.NewGuid(),
            bidderId: Guid.NewGuid(),
            paymentId: Guid.NewGuid(),
            bidValue: 100m,
            status: initialStatus,
            biddedAt: utcNow,
            outbiddedAt: null,
            wonAt: null);

        // Act
        Action act = () => bid.MarkAsOutbid(utcNow);

        // Assert
        act.Should().Throw<InvalidBidException>()
            .WithMessage("Only the winning bid can be marked as outbid.");
    }

    [Fact]
    public void SetAsWinner_WhenBidIsWinning_ShouldTransitionToWinner()
    {
        // Arrange
        var utcNow = _fixedNow;
        var bid = Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m, utcNow);
        var wonTime = utcNow.AddMinutes(5);

        // Act
        bid.SetAsWinner(wonTime);

        // Assert
        bid.Status.Should().Be(BidStatus.Winner);
        bid.WonAt.Should().Be(wonTime);
    }

    [Theory]
    [InlineData(BidStatus.Outbid)]
    [InlineData(BidStatus.Winner)]
    public void SetAsWinner_WhenBidIsNotWinning_ShouldThrowInvalidBidException(BidStatus initialStatus)
    {
        // Arrange
        var utcNow = _fixedNow;
        var bid = Bid.Restore(
            id: Guid.NewGuid(),
            auctionId: Guid.NewGuid(),
            bidderId: Guid.NewGuid(),
            paymentId: Guid.NewGuid(),
            bidValue: 100m,
            status: initialStatus,
            biddedAt: utcNow,
            outbiddedAt: null,
            wonAt: null);

        // Act
        Action act = () => bid.SetAsWinner(utcNow);

        // Assert
        act.Should().Throw<InvalidBidException>()
            .WithMessage("Only the winning bid can be marked as winner.");
    }
}