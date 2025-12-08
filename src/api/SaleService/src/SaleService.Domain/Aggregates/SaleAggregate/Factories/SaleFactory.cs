using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Aggregates.SaleAggregate.Enums;
using SalesService.Domain.Aggregates.SaleAggregate.Exceptions;

namespace SalesService.Domain.Aggregates.SaleAggregate.Factories;

public static class SaleFactory
{
    public static Sale Create(Guid productId, Guid buyerId, Guid sellerId, Guid listingId, Guid paymentId, decimal productValue)
    {
        if (productId == Guid.Empty)
            throw new InvalidSaleException("Product ID cannot be empty.");
        if (buyerId == Guid.Empty)
            throw new InvalidSaleException("Buyer ID cannot be empty.");
        if (sellerId == Guid.Empty)
            throw new InvalidSaleException("Seller ID cannot be empty.");
        if (listingId == Guid.Empty)
            throw new InvalidSaleException("Listing ID cannot be empty.");
        if (paymentId == Guid.Empty)
            throw new InvalidSaleException("Payment ID cannot be empty.");
        if (productValue <= 0)
            throw new InvalidSaleException("Product value must be greater than zero.");
        
        return new Sale(
            id: Guid.NewGuid(),
            productId: productId,
            buyerId: buyerId,
            sellerId: sellerId,
            listingId: listingId,
            paymentId: paymentId,
            productValue: productValue,
            createdAt: DateTime.UtcNow,
            updatedAt: DateTime.MinValue, 
            status: SaleStatus.AwaitingDelivery,
            deliveryStatus: DeliveryStatus.Pending, 
            deliveryCode: null, 
            isDeliveryCodeUsed: false,
            dispute: null
            );
    }

    public static Sale Load(
        Guid id, 
        Guid productId, 
        Guid buyerId, 
        Guid sellerId, 
        Guid listingId, 
        Guid paymentId, 
        decimal productValue, 
        SaleStatus status, 
        DeliveryStatus deliveryStatus, 
        string deliveryCode, 
        bool isDeliveryCodeUsed, 
        DateTime createdAt, 
        DateTime? updatedAt, 
        Dispute? dispute)
    {
        return new Sale(
            id: id,
            productId: productId,
            buyerId: buyerId,
            sellerId: sellerId,
            listingId: listingId,
            paymentId: paymentId,
            productValue: productValue,
            status: status,
            deliveryStatus: deliveryStatus, 
            deliveryCode: deliveryCode, 
            isDeliveryCodeUsed: isDeliveryCodeUsed,
            createdAt: createdAt,
            updatedAt: updatedAt,
            dispute: dispute);
    }
}