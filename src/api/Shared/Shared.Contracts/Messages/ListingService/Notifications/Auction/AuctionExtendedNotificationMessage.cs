namespace Shared.Contracts.Messages.ListingService.Notifications.Auction;

public record AuctionExtendedNotificationMessage(
    Guid ProductId,
    Guid SellerId,
    Guid BidderId,
    Guid? LastBidderId,
    int BidCount,
    DateTime NewEndDate);
