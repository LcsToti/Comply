namespace Shared.Contracts.Messages.ListingService.Common;

public record AuctionSettingsReadModelMessage(
    decimal StartBidValue,
    decimal WinBidValue,
    DateTime StartDate,
    DateTime EndDate);