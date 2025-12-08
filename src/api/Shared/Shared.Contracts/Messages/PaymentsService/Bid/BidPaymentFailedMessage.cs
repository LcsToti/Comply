namespace Shared.Contracts.Messages.PaymentsService.Bid;

public record BidPaymentFailedMessage(
    Guid AuctionId,
    Guid BidderId,
    DateTime ExpiresAt);