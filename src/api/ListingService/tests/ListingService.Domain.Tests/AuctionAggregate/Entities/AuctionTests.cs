using FluentAssertions;
using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.AuctionAggregate.Enums;
using ListingService.Domain.AuctionAggregate.ValueObjects;
using ListingService.Domain.Exceptions;

namespace ListingService.Domain.Tests.AuctionAggregate.Entities;

public class AuctionTests
{
    private readonly DateTime _fixedNow = new(2025, 10, 20, 10, 0, 0, DateTimeKind.Utc);

    private static AuctionSettings CreateDefaultSettings(DateTime utcNow)
    {
        return AuctionSettings.Create(
            startBidValue: 100,
            winBidValue: 1000,
            startDate: utcNow.AddHours(1),
            endDate: utcNow.AddDays(1),
            utcNow: utcNow
        );
    }

    [Fact]
    public void Create_WithValidParameters_ShouldCreateAwaitingAuction()
    {
        // Arrange
        var listingId = Guid.NewGuid();
        var utcNow = _fixedNow;
        var settings = CreateDefaultSettings(utcNow);

        // Act
        var auction = Auction.Create(listingId, settings);

        // Assert
        auction.Should().NotBeNull();
        auction.ListingId.Should().Be(listingId);
        auction.Settings.Should().Be(settings);
        auction.Status.Should().Be(AuctionStatus.Awaiting);
        auction.Version.Should().Be(1);
        auction.Bids.Should().BeEmpty();
        auction.IsProcessingBid.Should().BeFalse();
        auction.StartedAt.Should().BeNull();
        auction.EndedAt.Should().BeNull();
    }

    [Fact]
    public void Start_WhenAuctionIsAwaiting_ShouldTransitionToActive()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        var initialVersion = auction.Version;

        // Act
        auction.Start(utcNow);

        // Assert
        auction.Status.Should().Be(AuctionStatus.Active);
        auction.StartedAt.Should().Be(utcNow);
        auction.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void Start_WhenAuctionIsNotAwaiting_ShouldThrowInvalidAuctionException()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        auction.Start(utcNow); // Status is now Active

        // Act
        Action act = () => auction.Start(utcNow);

        // Assert
        act.Should().Throw<InvalidAuctionException>()
            .WithMessage("Its only possible to start an awaiting auction.");
    }

    [Fact]
    public void SetAsEnding_WhenAuctionIsActive_ShouldTransitionToEnding()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        auction.Start(utcNow); // Must be active to be set as ending
        var initialVersion = auction.Version;

        // Act
        auction.SetAsEnding();

        // Assert
        auction.Status.Should().Be(AuctionStatus.Ending);
        auction.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void SetAsEnding_WhenAuctionIsNotActive_ShouldThrowInvalidAuctionException()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow)); // Status is Awaiting

        // Act
        Action act = () => auction.SetAsEnding();

        // Assert
        act.Should().Throw<InvalidAuctionException>()
            .WithMessage("Only an active auction can be set as ending.");
    }

    [Fact]
    public void Finish_WhenActiveAndHasWinningBid_ShouldTransitionToSuccess()
    {
        // Arrange
        var utcNow = _fixedNow;
        var settings = CreateDefaultSettings(utcNow);
        var bid = Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 500, utcNow);

        var auction = Auction.Restore(
            id: Guid.NewGuid(),
            listingId: Guid.NewGuid(),
            settings: settings,
            status: AuctionStatus.Active,
            version: 2,
            isProcessingBid: false,
            editedAt: null,
            startedAt: utcNow.AddMinutes(-10),
            endedAt: null,
            bids: [bid]
        );
        var initialVersion = auction.Version;

        // Act
        auction.Finish(utcNow);

        // Assert
        auction.Status.Should().Be(AuctionStatus.Success);
        auction.EndedAt.Should().Be(utcNow);
        auction.Version.Should().Be(initialVersion + 1);
        auction.Bids.First(b => b.Status == BidStatus.Winner).Should().NotBeNull();
    }

    [Fact]
    public void Finish_WhenActiveAndHasNoBids_ShouldTransitionToFailed()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        auction.Start(utcNow);
        var initialVersion = auction.Version;

        // Act
        auction.Finish(utcNow);

        // Assert
        auction.Status.Should().Be(AuctionStatus.Failed);
        auction.EndedAt.Should().Be(utcNow);
        auction.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void Finish_WhenStatusIsNotActiveOrEnding_ShouldThrowInvalidAuctionException()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow)); // Status is Awaiting

        // Act
        Action act = () => auction.Finish(utcNow);

        // Assert
        act.Should().Throw<InvalidAuctionException>()
            .WithMessage("Only Active or Ending auctions can be finished.");
    }

    [Fact]
    public void Cancel_WhenActiveAndHasNoBids_ShouldTransitionToCancelled()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        auction.Start(utcNow);
        var initialVersion = auction.Version;

        // Act
        auction.Cancel(utcNow);

        // Assert
        auction.Status.Should().Be(AuctionStatus.Cancelled);
        auction.EndedAt.Should().Be(utcNow);
        auction.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void Cancel_WhenAuctionHasBids_ShouldThrowInvalidAuctionException()
    {
        // Arrange
        var utcNow = _fixedNow;
        var settings = CreateDefaultSettings(utcNow);
        var bid = Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 200, utcNow);

        var auction = Auction.Restore(
            id: Guid.NewGuid(),
            listingId: Guid.NewGuid(),
            settings: settings,
            status: AuctionStatus.Active,
            version: 2,
            isProcessingBid: false,
            editedAt: null,
            startedAt: utcNow.AddMinutes(-10),
            endedAt: null,
            bids: [bid]
        );

        // Act
        Action act = () => auction.Cancel(utcNow);

        // Assert
        act.Should().Throw<InvalidAuctionException>()
            .WithMessage("It's not possible to cancel an auction that has bids");
    }

    [Fact]
    public void Cancel_WhenStatusIsNotActiveOrEnding_ShouldThrowInvalidAuctionException()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow)); // Status is Awaiting

        // Act
        Action act = () => auction.Cancel(utcNow);

        // Assert
        act.Should().Throw<InvalidAuctionException>()
            .WithMessage("Its only possible to cancel an active/ending auction.");
    }

    [Fact]
    public void EditSettings_WhenAuctionIsAwaiting_ShouldUpdateSettingsAndVersion()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        var initialVersion = auction.Version;
        var newStartBid = 200m;
        var newEndDate = utcNow.AddDays(5);

        // Act
        auction.EditSettings(utcNow, startBidValue: newStartBid, endDate: newEndDate);

        // Assert
        auction.Settings.StartBidValue.Should().Be(newStartBid);
        auction.Settings.EndDate.Should().Be(newEndDate);
        auction.EditedAt.Should().Be(utcNow);
        auction.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void EditSettings_WhenAuctionIsNotAwaiting_ShouldThrowInvalidAuctionException()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        auction.Start(utcNow); // Status: Active

        // Act
        Action act = () => auction.EditSettings(utcNow, startBidValue: 200m);

        // Assert
        act.Should().Throw<InvalidAuctionException>()
            .WithMessage("Only Awaiting auctions can have their settings edited.");
    }

    [Fact]
    public void PrepareNewBid_WhenBidIsBelowMinimum_ShouldThrowInvalidBidException()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        auction.Start(utcNow);
        var minimumBid = auction.GetMinimumNextBidValue(); // Should be 100
        var invalidBidValue = minimumBid - 1;

        // Act
        Action act = () => auction.PrepareNewBid(invalidBidValue);

        // Assert
        act.Should().Throw<InvalidBidException>()
            .WithMessage($"Your bid must be at least {minimumBid:C}.");
    }

    [Fact]
    public void AbortNewBid_WhenBidIsBeingProcessed_ShouldResetProcessingFlag()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));

        auction.Start(utcNow);
        auction.PrepareNewBid(150m);
        typeof(Auction).GetProperty(nameof(Auction.IsProcessingBid))!.SetValue(auction, true); // isProcessingBid is now true

        // Act
        auction.AbortNewBid();

        // Assert
        auction.IsProcessingBid.Should().BeFalse();
    }

    [Fact]
    public void AbortNewBid_WhenNoBidIsBeingProcessed_ShouldThrowException()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        auction.Start(utcNow); // isProcessingBid is false

        // Act
        Action act = () => auction.AbortNewBid();

        // Assert
        act.Should().Throw<Exception>().WithMessage("Cant abort a new bid when there is no bid being processed.");
    }

    [Fact]
    public void CompleteNewBid_WhenBidIsBeingProcessed_ShouldAddBidAndResetFlag()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        auction.Start(utcNow);
        auction.PrepareNewBid(150m); 
        typeof(Auction).GetProperty(nameof(Auction.IsProcessingBid))!.SetValue(auction, true); // isProcessingBid is now true
        var bidderId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        var bidValue = 150m;

        // Act
        auction.CompleteNewBid(bidderId, paymentId, bidValue, utcNow);

        // Assert
        auction.IsProcessingBid.Should().BeFalse();
        auction.Bids.Should().HaveCount(1);
        auction.Bids.First().Value.Should().Be(bidValue);
        auction.Bids.First().BidderId.Should().Be(bidderId);
    }

    [Fact]
    public void ExtendAuction_WhenCalled_ShouldUpdateSettingsEndDateAndVersion()
    {
        // Arrange
        var utcNow = _fixedNow;
        var auction = Auction.Create(Guid.NewGuid(), CreateDefaultSettings(utcNow));
        var initialVersion = auction.Version;
        var initialEndDate = auction.Settings.EndDate;

        // Act
        auction.ExtendAuction();

        // Assert
        auction.Settings.EndDate.Should().BeAfter(initialEndDate);
        auction.Version.Should().Be(initialVersion + 1);
    }

    [Fact]
    public void GetMinimumNextBidValue_WhenNoBidsExist_ShouldReturnStartBidValue()
    {
        // Arrange
        var utcNow = _fixedNow;
        var settings = CreateDefaultSettings(utcNow);
        var auction = Auction.Create(Guid.NewGuid(), settings);

        // Act
        var minimumBid = auction.GetMinimumNextBidValue();

        // Assert
        minimumBid.Should().Be(settings.StartBidValue);
    }

    [Fact]
    public void GetMinimumNextBidValue_WhenBidsExist_ShouldReturnCalculatedNextBid()
    {
        // Arrange
        var utcNow = _fixedNow;
        var settings = CreateDefaultSettings(utcNow);
        var winningBidValue = 200m;
        var bid = Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), winningBidValue, utcNow);

        var auction = Auction.Restore(
            id: Guid.NewGuid(),
            listingId: Guid.NewGuid(),
            settings: settings,
            status: AuctionStatus.Active,
            version: 2,
            isProcessingBid: false,
            editedAt: null,
            startedAt: utcNow.AddMinutes(-10),
            endedAt: null,
            bids: [bid]
        );
        var expectedNextBid = Math.Ceiling(winningBidValue * 1.05m); // 200 * 1.05 = 210

        // Act
        var minimumBid = auction.GetMinimumNextBidValue();

        // Assert
        minimumBid.Should().Be(expectedNextBid);
    }

    [Fact]
    public void GetMinimumNextBidValue_WhenCalculatedBidExceedsWinBid_ShouldReturnWinBidValue()
    {
        // Arrange
        var utcNow = _fixedNow;
        var settings = CreateDefaultSettings(utcNow); // WinBidValue is 1000
        var winningBidValue = 980m; // 980 * 1.05 = 1029
        var bid = Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), winningBidValue, utcNow);

        var auction = Auction.Restore(
            id: Guid.NewGuid(),
            listingId: Guid.NewGuid(),
            settings: settings,
            status: AuctionStatus.Active,
            version: 2,
            isProcessingBid: false,
            editedAt: null,
            startedAt: utcNow.AddMinutes(-10),
            endedAt: null,
            bids: [bid]
        );

        // Act
        var minimumBid = auction.GetMinimumNextBidValue();

        // Assert
        minimumBid.Should().Be(settings.WinBidValue);
    }

    [Theory]
    [InlineData(0, 100, 1000, 100)] // Scenario 1: No bids, should return the starting price
    [InlineData(200, 100, 1000, 210)] // Scenario 2: Current bid is 200, next should be 210 (200 * 1.05)
    [InlineData(199.50, 100, 1000, 210)] // Scenario 3: Calculation with rounding up   
    [InlineData(980, 100, 1000, 1000)] // Scenario 4: Calculation exceeds the buyout price, should be capped at the buyout value
    public void GetMinimumNextBidValue_WithVariousScenarios_ShouldReturnCorrectValue(
    decimal currentWinningBidValue,
    decimal startBid,
    decimal winBid,
    decimal expectedNextBid)
    {
        // Arrange
        var utcNow = _fixedNow;
        var settings = AuctionSettings.Create(startBid, winBid, utcNow.AddHours(1), utcNow.AddDays(1), utcNow);

        var bids = new List<Bid>();
        if (currentWinningBidValue > 0)
        {
            var winningBid = Bid.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), currentWinningBidValue, utcNow);
            bids.Add(winningBid);
        }

        var auction = Auction.Restore(Guid.NewGuid(), Guid.NewGuid(), settings, AuctionStatus.Active, 1, false, null, utcNow, null, bids);

        // Act
        var minimumBid = auction.GetMinimumNextBidValue();

        // Assert
        minimumBid.Should().Be(expectedNextBid);
    }
}