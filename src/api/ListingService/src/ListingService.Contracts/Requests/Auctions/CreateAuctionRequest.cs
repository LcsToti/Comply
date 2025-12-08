namespace ListingService.Contracts.Requests.Auctions;

public record CreateAuctionRequest(
    Guid ListingId,
    decimal StartBidValue,
    decimal WinBidValue,
    DateTime StartDate,
    DateTime EndDate);