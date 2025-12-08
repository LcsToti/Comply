using SalesService.Domain.Aggregates.SaleAggregate.Entities;

namespace SalesService.App.Common.Results.Mappers;

public static class ToSaleResultMapper
{
    public static SaleResult ToSaleResult(this Sale sale)
    {
        return new SaleResult(
            sale.Id,
            sale.ProductId,
            sale.ListingId,
            sale.PaymentId,
            sale.BuyerId,
            sale.SellerId,
            sale.ProductValue,
            sale.CreatedAt,
            sale.UpdatedAt,
            sale.Status,
            sale.DeliveryStatus,
            sale.Dispute
        );
    }
}