namespace Shared.Contracts.Messages.ListingService.Sales;

public record CreateSaleMessage(
    Guid ProductId, 
    Guid BuyerId, 
    Guid SellerId, 
    Guid ListingId, 
    Guid PaymentId,
    decimal ProductValue);