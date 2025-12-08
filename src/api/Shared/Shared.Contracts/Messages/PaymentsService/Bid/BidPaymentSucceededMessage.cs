namespace Shared.Contracts.Messages.PaymentsService.Bid;

public record BidPaymentSucceededMessage(
    Guid AuctionId,
    Guid BidderId,
    decimal BidValue,
    DateTime ExpiresAt,
    Guid PaymentId);