namespace Shared.Contracts.Messages.ListingService.Payments.Bid;

public record BidRefundRequestMessage(
    decimal AmountToRefund, 
    Guid PaymentId, 
    string Reason, 
    Guid UserId);