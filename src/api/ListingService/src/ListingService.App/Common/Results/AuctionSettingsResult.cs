namespace ListingService.App.Common.Results;

public record AuctionSettingsResult(
    decimal StartBidValue,
    decimal WinBidValue,
    DateTime StartDate,
    DateTime EndDate);