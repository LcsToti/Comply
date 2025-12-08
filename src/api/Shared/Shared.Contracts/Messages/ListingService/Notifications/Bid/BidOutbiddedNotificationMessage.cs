namespace Shared.Contracts.Messages.ListingService.Notifications.Bid;

public record BidOutbiddedNotificationMessage(
    Guid ProductId,
    Guid SellerId,
    Guid NewBidderId, 
    Guid LastBidderId,
    decimal NewBidValue);