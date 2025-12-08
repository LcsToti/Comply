namespace ListingService.App.Messages.AuctionStateMessages;

public record SetAuctionAsEndingMessage(
    Guid AuctionId,
    int Version);