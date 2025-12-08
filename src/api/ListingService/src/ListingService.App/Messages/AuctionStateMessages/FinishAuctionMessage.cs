namespace ListingService.App.Messages.AuctionStateMessages;

public record FinishAuctionMessage(
    Guid AuctionId,
    int Version);