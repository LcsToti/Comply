using ListingService.Domain.Common;
using ListingService.Domain.Exceptions;

namespace ListingService.Domain.AuctionAggregate.ValueObjects;

public record AuctionSettings
{
    public decimal StartBidValue { get; init; }
    public decimal WinBidValue { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    #region For ORM
    private AuctionSettings() { }

    public static AuctionSettings Restore(decimal startBidValue, decimal winBidValue, DateTime startDate, DateTime endDate)
    {
        return new AuctionSettings
        {
            WinBidValue = winBidValue,
            StartBidValue = startBidValue,
            StartDate = startDate,
            EndDate = endDate
        };
    }
    #endregion

    /// <summary>Initializes a new instance of AuctionSettings with validation.</summary>
    /// <exception cref="InvalidAuctionSettingsException">Thrown if any of the provided values are outside valid auction settings.</exception>
    private AuctionSettings(decimal startBidValue, decimal winBidValue, DateTime startDate, DateTime endDate)
    {
        WinBidValue = winBidValue;
        StartBidValue = startBidValue;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static AuctionSettings Create(decimal startBidValue, decimal winBidValue, DateTime startDate, DateTime endDate, DateTime utcNow)
    {
        if (startBidValue < DomainConstants.MinTransactionValue)
            throw new InvalidAuctionSettingsException($"Start bid value must be at least {DomainConstants.MinTransactionValue:C}.");
        if (startBidValue > DomainConstants.MaxTransactionValue)
            throw new InvalidAuctionSettingsException($"Start bid value must not exceed {DomainConstants.MaxTransactionValue:C}.");

        decimal minAutoWin = startBidValue * 1.2m;
        if (winBidValue < minAutoWin)
            throw new InvalidAuctionSettingsException($"WinBidValue must be at least 20% greater than StartBidValue {startBidValue:C}, that is, at least {minAutoWin:C}.");

        if (winBidValue > DomainConstants.MaxTransactionValue)
            throw new InvalidAuctionSettingsException($"WinBidValue must not exceed {DomainConstants.MaxTransactionValue:C}.");

        if (startDate < utcNow.AddMinutes(DomainConstants.MinMinutesBeforeAuctionStarts))
            throw new InvalidAuctionSettingsException($"Start date must be at least {DomainConstants.MinMinutesBeforeAuctionStarts} minutes in the future.");

        if (endDate < startDate.AddMinutes(DomainConstants.MinAuctionDurationMinutes))
            throw new InvalidAuctionSettingsException($"End date must be at least {DomainConstants.MinAuctionDurationMinutes} minutes after start date.");

        return new AuctionSettings(startBidValue, winBidValue, startDate, endDate);
    }
    public AuctionSettings ExtendEndDate()
    {
        var newEndDate = EndDate + DomainConstants.RemainingTimeToSetEnding;
        return new AuctionSettings(StartBidValue, WinBidValue, StartDate, newEndDate);
    }
}

