namespace Shared.Contracts.Messages.PaymentsService.Bid;

public record BidPaymentRefundFailedMessage(Guid AuctionId, Guid BidderId);