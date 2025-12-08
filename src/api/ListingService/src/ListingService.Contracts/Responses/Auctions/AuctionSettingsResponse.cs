namespace ListingService.Contracts.Responses.Auctions;

public record AuctionSettingsResponse(
    decimal StartBidValue,
    decimal WinBidValue,
    DateTime StartDate,
    DateTime EndDate);
