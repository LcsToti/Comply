namespace Shared.Contracts.Messages.PaymentsService.Bid;

public record BidPaymentRefundedMessage(Guid AuctionId, Guid BidderId);