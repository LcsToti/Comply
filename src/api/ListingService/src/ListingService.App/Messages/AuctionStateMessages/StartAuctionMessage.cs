namespace ListingService.App.Messages.AuctionStateMessages;

public record StartAuctionMessage(
    Guid AuctionId,
    int Version);