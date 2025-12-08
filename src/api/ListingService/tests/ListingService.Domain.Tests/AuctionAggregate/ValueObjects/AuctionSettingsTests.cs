using FluentAssertions;
using ListingService.Domain.AuctionAggregate.ValueObjects;
using ListingService.Domain.Common;
using ListingService.Domain.Exceptions;

namespace ListingService.Domain.Tests.AuctionAggregate.ValueObjects;

public class AuctionSettingsTests
{
    private readonly DateTime _fixedNow = new(2025, 10, 20, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var startBidValue = 100m;
        var winBidValue = 200m;
        var startDate = _fixedNow.AddMinutes(DomainConstants.MinMinutesBeforeAuctionStarts);
        var endDate = startDate.AddMinutes(DomainConstants.MinAuctionDurationMinutes);

        // Act
        var settings = AuctionSettings.Create(startBidValue, winBidValue, startDate, endDate, _fixedNow);

        // Assert
        settings.Should().NotBeNull();
        settings.StartBidValue.Should().Be(startBidValue);
        settings.WinBidValue.Should().Be(winBidValue);
        settings.StartDate.Should().Be(startDate);
        settings.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void Create_WhenStartBidIsTooLow_ShouldThrowInvalidAuctionSettingsException()
    {
        // Arrange
        var invalidStartBid = DomainConstants.MinTransactionValue - 1;

        // Act
        Action act = () => AuctionSettings.Create(
            invalidStartBid,
            200m,
            _fixedNow.AddMinutes(10),
            _fixedNow.AddHours(1),
            _fixedNow);

        // Assert
        act.Should().Throw<InvalidAuctionSettingsException>()
            .WithMessage($"Start bid value must be at least {DomainConstants.MinTransactionValue:C}.");
    }

    [Fact]
    public void Create_WhenWinBidIsInsufficient_ShouldThrowInvalidAuctionSettingsException()
    {
        // Arrange
        var startBidValue = 100m;
        var insufficientWinBid = startBidValue * 1.1m; // Menos que os 20% necessários
        var minAutoWin = startBidValue * 1.2m;

        // Act
        Action act = () => AuctionSettings.Create(
            startBidValue,
            insufficientWinBid,
            _fixedNow.AddMinutes(10),
            _fixedNow.AddHours(1),
            _fixedNow);

        // Assert
        act.Should().Throw<InvalidAuctionSettingsException>()
            .WithMessage($"WinBidValue must be at least 20% greater than StartBidValue {startBidValue:C}, that is, at least {minAutoWin:C}.");
    }

    [Fact]
    public void Create_WhenStartDateIsTooSoon_ShouldThrowInvalidAuctionSettingsException()
    {
        // Arrange
        var invalidStartDate = _fixedNow.AddMinutes(DomainConstants.MinMinutesBeforeAuctionStarts - 1);

        // Act
        Action act = () => AuctionSettings.Create(
            100m,
            200m,
            invalidStartDate,
            _fixedNow.AddHours(1),
            _fixedNow);

        // Assert
        act.Should().Throw<InvalidAuctionSettingsException>()
            .WithMessage($"Start date must be at least {DomainConstants.MinMinutesBeforeAuctionStarts} minutes in the future.");
    }

    [Fact]
    public void Create_WhenDurationIsTooShort_ShouldThrowInvalidAuctionSettingsException()
    {
        // Arrange
        var startDate = _fixedNow.AddMinutes(10);
        var invalidEndDate = startDate.AddMinutes(DomainConstants.MinAuctionDurationMinutes - 1);

        // Act
        Action act = () => AuctionSettings.Create(
            100m,
            200m,
            startDate,
            invalidEndDate,
            _fixedNow);

        // Assert
        act.Should().Throw<InvalidAuctionSettingsException>()
            .WithMessage($"End date must be at least {DomainConstants.MinAuctionDurationMinutes} minutes after start date.");
    }

    [Fact]
    public void ExtendEndDate_WhenCalled_ReturnsNewInstanceWithExtendedDate()
    {
        // Arrange
        var startDate = _fixedNow.AddMinutes(10);
        var endDate = startDate.AddMinutes(30);
        var settings = AuctionSettings.Restore(100m, 200m, startDate, endDate);
        var expectedNewEndDate = endDate + DomainConstants.RemainingTimeToSetEnding;

        // Act
        var newSettings = settings.ExtendEndDate();

        // Assert
        newSettings.Should().NotBeSameAs(settings); // Deve ser uma nova instância
        newSettings.EndDate.Should().Be(expectedNewEndDate);
        newSettings.StartDate.Should().Be(settings.StartDate);
        newSettings.StartBidValue.Should().Be(settings.StartBidValue);
        newSettings.WinBidValue.Should().Be(settings.WinBidValue);
    }

    [Fact]
    public void Restore_WithAnyValues_ShouldCreateInstanceWithoutValidation()
    {
        // Arrange
        // Valores que falhariam na validação do Create()
        var startBidValue = 0m;
        var winBidValue = 0m;
        var startDate = _fixedNow.AddDays(-1);
        var endDate = _fixedNow;

        // Act
        var settings = AuctionSettings.Restore(startBidValue, winBidValue, startDate, endDate);

        // Assert
        settings.Should().NotBeNull();
        settings.StartBidValue.Should().Be(startBidValue);
        settings.WinBidValue.Should().Be(winBidValue);
        settings.StartDate.Should().Be(startDate);
        settings.EndDate.Should().Be(endDate);
    }
}