using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Aggregates.SaleAggregate.Factories;
using SalesService.Infra.Persistence.Document;

namespace SalesService.Infra.Persistence.Mappers;

public static class SaleMapper
{
    public static Sale ToDomain(this SaleDocument saleDocument)
    {
        Dispute? dispute = null;

        if (saleDocument.Dispute != null)
        {
            dispute = Dispute.Load(
                saleDocument.Dispute.Id,
                saleDocument.Dispute.AdminId,
                saleDocument.Dispute.Status,
                saleDocument.Dispute.ResolutionStatus,
                saleDocument.Dispute.Reason,
                saleDocument.Dispute.Resolution,
                saleDocument.Dispute.CreatedAt,
                saleDocument.Dispute.UpdatedAt,
                saleDocument.Dispute.ResolvedAt,
                saleDocument.Dispute.ExpiresAt
            );
        }

        return SaleFactory.Load(
            saleDocument.Id,
            saleDocument.ProductId,
            saleDocument.BuyerId,
            saleDocument.SellerId,
            saleDocument.ListingId,
            saleDocument.PaymentId,
            saleDocument.ProductValue,
            saleDocument.Status,
            saleDocument.DeliveryStatus,
            saleDocument.DeliveryCode ?? string.Empty,
            saleDocument.IsDeliveryCodeUsed,
            saleDocument.CreatedAt,
            saleDocument.UpdatedAt,
            dispute
        );
    }
    
    public static SaleDocument ToDocument(Sale sale) => new ()
    {
        Id = sale.Id,
        ProductId = sale.ProductId,
        BuyerId = sale.BuyerId,
        SellerId = sale.SellerId,
        ListingId = sale.ListingId,
        PaymentId = sale.PaymentId,
        ProductValue = sale.ProductValue,
        Status = sale.Status,
        DeliveryStatus = sale.DeliveryStatus,
        DeliveryCode = sale.DeliveryCode,
        IsDeliveryCodeUsed = sale.IsDeliveryCodeUsed,
        CreatedAt = sale.CreatedAt,
        UpdatedAt = sale.UpdatedAt,
        Dispute = sale.Dispute == null ? null : new DisputeDocument
        {
            Id = sale.Dispute.Id,
            AdminId = sale.Dispute.AdminId,
            Status = sale.Dispute.Status,
            ResolutionStatus = sale.Dispute.ResolutionStatus,
            Reason = sale.Dispute.Reason,
            Resolution = sale.Dispute.Resolution,
            CreatedAt = sale.Dispute.CreatedAt,
            UpdatedAt = sale.Dispute.UpdatedAt,
            ResolvedAt = sale.Dispute.ResolvedAt,
            ExpiresAt = sale.Dispute.ExpiresAt,
        }
    };   
}