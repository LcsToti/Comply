namespace Shared.Contracts.Messages.ListingService.Notifications.Auction;

public record AuctionEndingNotificationMessage(
    Guid ProductId,
    Guid SellerId,
    int BidCount,
    Guid? LastBidderId);
