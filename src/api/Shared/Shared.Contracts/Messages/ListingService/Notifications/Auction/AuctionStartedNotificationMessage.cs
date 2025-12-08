namespace Shared.Contracts.Messages.ListingService.Notifications.Auction;

public record AuctionStartedNotificationMessage(
    Guid ProductId,
    Guid SellerId,
    DateTime StartedAt,
    decimal MinBidValue);
