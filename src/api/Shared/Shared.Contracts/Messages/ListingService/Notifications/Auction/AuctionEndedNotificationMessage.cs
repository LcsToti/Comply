namespace Shared.Contracts.Messages.ListingService.Notifications.Auction;

public record AuctionEndedNotificationMessage(
    Guid ProductId, 
    Guid SellerId,
    int BidCount,
    string Status,
    Guid? WinnerId,
    decimal? FinalValue);