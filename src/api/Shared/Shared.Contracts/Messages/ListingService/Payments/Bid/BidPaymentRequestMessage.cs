namespace Shared.Contracts.Messages.ListingService.Payments.Bid;

public record BidPaymentRequestMessage(
    Guid AuctionId,
    Guid BidderId,
    DateTime ExpiresAt,
    decimal Value,
    string PaymentMethod,
    Guid SellerId);
