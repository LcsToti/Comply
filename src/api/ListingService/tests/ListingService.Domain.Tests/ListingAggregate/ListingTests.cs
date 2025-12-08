using FluentAssertions;
using ListingService.Domain.Common;
using ListingService.Domain.Exceptions;
using ListingService.Domain.ListingAggregate;

namespace ListingService.Domain.Tests.ListingAggregate;

public class ListingTests
{
    private readonly DateTime _fixedNow = new(2025, 10, 20, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithValidParameters_ShouldCreateAvailableListing()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var buyPrice = 150m;

        // Act
        var listing = Listing.Create(sellerId, productId, buyPrice, _fixedNow);

        // Assert
        listing.Should().NotBeNull();
        listing.SellerId.Should().Be(sellerId);
        listing.ProductId.Should().Be(productId);
        listing.BuyPrice.Should().Be(buyPrice);
        listing.Status.Should().Be(ListingStatus.Available);
        listing.IsAuctionActive.Should().BeFalse();
        listing.IsProcessingPurchase.Should().BeFalse();
        listing.ListedAt.Should().Be(_fixedNow);
        listing.UpdatedAt.Should().Be(_fixedNow);
    }

    [Fact]
    public void Create_WithPriceBelowMinimum_ShouldThrowInvalidListingException()
    {
        // Arrange
        var invalidPrice = DomainConstants.MinTransactionValue - 1;

        // Act
        Action act = () => Listing.Create(Guid.NewGuid(), Guid.NewGuid(), invalidPrice, _fixedNow);

        // Assert
        act.Should().Throw<InvalidListingException>()
            .WithMessage($"Buy price should be at least {DomainConstants.MinTransactionValue:C}.");
    }

    [Fact]
    public void UpdateBuyPrice_WhenListingIsAvailable_ShouldUpdatePriceAndDate()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        var newPrice = 200m;
        var updateTime = _fixedNow.AddMinutes(10);

        // Act
        listing.UpdateBuyPrice(newPrice, updateTime);

        // Assert
        listing.BuyPrice.Should().Be(newPrice);
        listing.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void UpdateBuyPrice_WhenListingIsSold_ShouldThrowInvalidListingException()
    {
        // Arrange
        var listing = Listing.Restore(
            id: Guid.NewGuid(),
            sellerId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            status: ListingStatus.Sold, // Estado inválido para a operação
            buyPrice: 150m,
            isAuctionActive: false,
            isProcessingPurchase: false,
            buyerId: Guid.NewGuid(),
            auctionId: null,
            listedAt: _fixedNow,
            updatedAt: _fixedNow
        );

        // Act
        Action act = () => listing.UpdateBuyPrice(200m, _fixedNow.AddMinutes(5));

        // Assert
        act.Should().Throw<InvalidListingException>()
            .WithMessage("It is only possible to update a buy price of a listing if its Available or Paused.");
    }

    [Fact]
    public void UpdateBuyPrice_WhenAuctionIsActive_ShouldThrowInvalidListingException()
    {
        // Arrange
        var listing = Listing.Restore(
            id: Guid.NewGuid(),
            sellerId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            status: ListingStatus.Available,
            buyPrice: 150m,
            isAuctionActive: true, // Estado inválido para a operação
            isProcessingPurchase: false,
            buyerId: null,
            auctionId: Guid.NewGuid(),
            listedAt: _fixedNow,
            updatedAt: _fixedNow
        );

        // Act
        Action act = () => listing.UpdateBuyPrice(200m, _fixedNow.AddMinutes(5));

        // Assert
        act.Should().Throw<InvalidListingException>()
            .WithMessage("It is not possible to change the buy price during an auction.");
    }

    [Fact]
    public void ToggleAvailability_FromAvailableToPaused_ShouldUpdateStatus()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        var updateTime = _fixedNow.AddHours(1);

        // Act
        listing.ToggleAvailability(updateTime);

        // Assert
        listing.Status.Should().Be(ListingStatus.Paused);
        listing.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void ToggleAvailability_WhenListingHasAuction_ShouldThrowInvalidListingException()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        listing.AttachAuction(Guid.NewGuid(), _fixedNow); // Anexa um leilão

        // Act
        Action act = () => listing.ToggleAvailability(_fixedNow.AddMinutes(5));

        // Assert
        act.Should().Throw<InvalidListingException>()
            .WithMessage("It is not possible to change the visibility of a listing with an auction.");
    }

    [Theory]
    [InlineData(ListingStatus.Paused)]
    [InlineData(ListingStatus.Sold)]
    [InlineData(ListingStatus.SoldByAuction)]
    public void PrepareBuyNow_WhenStatusIsNotAvailable_ShouldThrowInvalidListingException(ListingStatus status)
    {
        // Arrange
        var listing = Listing.Restore(
            id: Guid.NewGuid(), sellerId: Guid.NewGuid(), productId: Guid.NewGuid(),
            status: status,
            buyPrice: 150m, isAuctionActive: false, isProcessingPurchase: false,
            buyerId: null, auctionId: null, listedAt: _fixedNow, updatedAt: _fixedNow
        );

        // Act
        Action act = () => listing.PrepareBuyNow();

        // Assert
        act.Should().Throw<InvalidListingException>()
            .WithMessage("Product is not available for sold.");
    }

    [Fact]
    public void AbortBuyNow_WhenPurchaseIsBeingProcessed_ShouldResetFlag()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        listing.PrepareBuyNow();
        typeof(Listing).GetProperty(nameof(Listing.IsProcessingPurchase))!.SetValue(listing, true);

        // Act
        listing.AbortBuyNow();

        // Assert
        listing.IsProcessingPurchase.Should().BeFalse();
    }

    [Fact]
    public void AbortBuyNow_WhenPurchaseIsNotBeingProcessed_ShouldThrowException()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        // IsProcessingPurchase is false by default

        // Act
        Action act = () => listing.AbortBuyNow();

        // Assert
        act.Should().Throw<Exception>()
            .WithMessage("It is not possible to abort the purchase of a product that is not being processed.");
    }

    [Fact]
    public void CompleteBuyNow_WhenPurchaseIsBeingProcessed_ShouldMarkAsSold()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        listing.PrepareBuyNow();
        typeof(Listing).GetProperty(nameof(Listing.IsProcessingPurchase))!.SetValue(listing, true);
        var buyerId = Guid.NewGuid();
        var boughtAt = _fixedNow.AddMinutes(1);

        // Act
        listing.CompleteBuyNow(buyerId, boughtAt);

        // Assert
        listing.IsProcessingPurchase.Should().BeFalse();
        listing.Status.Should().Be(ListingStatus.Sold);
        listing.BuyerId.Should().Be(buyerId);
        listing.UpdatedAt.Should().Be(boughtAt);
    }

    [Fact]
    public void AttachAuction_WhenListingIsAvailable_ShouldSetAuctionId()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        var auctionId = Guid.NewGuid();
        var updateTime = _fixedNow.AddMinutes(5);

        // Act
        listing.AttachAuction(auctionId, updateTime);

        // Assert
        listing.AuctionId.Should().Be(auctionId);
        listing.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void AttachAuction_WhenAlreadyHasAuction_ShouldThrowInvalidListingException()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        listing.AttachAuction(Guid.NewGuid(), _fixedNow);

        // Act
        Action act = () => listing.AttachAuction(Guid.NewGuid(), _fixedNow.AddMinutes(1));

        // Assert
        act.Should().Throw<InvalidListingException>()
            .WithMessage("It is not possible to attach an auction to a product that is already in auction.");
    }

    [Fact]
    public void DetachAuction_WhenAuctionIsAttachedAndInactive_ShouldRemoveAuctionId()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        listing.AttachAuction(Guid.NewGuid(), _fixedNow);
        var updateTime = _fixedNow.AddHours(1);

        // Act
        listing.DetachAuction(updateTime);

        // Assert
        listing.AuctionId.Should().BeNull();
        listing.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void SetAuctionActive_WhenAvailableWithAuction_ShouldSetFlag()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        listing.AttachAuction(Guid.NewGuid(), _fixedNow);
        var updateTime = _fixedNow.AddHours(1);

        // Act
        listing.SetAuctionActive(updateTime);

        // Assert
        listing.IsAuctionActive.Should().BeTrue();
        listing.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void SetAuctionInactive_WhenAuctionIsActive_ShouldResetFlag()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        listing.AttachAuction(Guid.NewGuid(), _fixedNow);
        listing.SetAuctionActive(_fixedNow.AddMinutes(5));
        var updateTime = _fixedNow.AddHours(1);

        // Act
        listing.SetAuctionInactive(updateTime);

        // Assert
        listing.IsAuctionActive.Should().BeFalse();
        listing.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void MarkAsSoldByAuction_WhenAuctionIsAttached_ShouldUpdateStatusAndBuyer()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);
        listing.AttachAuction(Guid.NewGuid(), _fixedNow);
        var winnerId = Guid.NewGuid();
        var soldAt = _fixedNow.AddDays(1);

        // Act
        listing.MarkAsSoldByAuction(winnerId, soldAt);

        // Assert
        listing.Status.Should().Be(ListingStatus.SoldByAuction);
        listing.BuyerId.Should().Be(winnerId);
        listing.UpdatedAt.Should().Be(soldAt);
    }

    [Fact]
    public void MarkAsSoldByAuction_WhenNoAuctionIsAttached_ShouldThrowInvalidListingException()
    {
        // Arrange
        var listing = Listing.Create(Guid.NewGuid(), Guid.NewGuid(), 150m, _fixedNow);

        // Act
        Action act = () => listing.MarkAsSoldByAuction(Guid.NewGuid(), _fixedNow.AddDays(1));

        // Assert
        act.Should().Throw<InvalidListingException>()
            .WithMessage("It is not possible to sell a product via auction if it is not attached to an auction.");
    }
}