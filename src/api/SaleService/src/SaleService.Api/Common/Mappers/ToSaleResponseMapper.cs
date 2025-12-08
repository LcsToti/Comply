using SalesService.API.Responses;
using SalesService.App.Common.Results;

namespace SalesService.API.Common.Mappers;

public static class ToSaleResponseMapper
{
    public static SaleResponse ToSaleResponse(this SaleResult result)
    {
        return new SaleResponse(
            result.Id,
            result.ProductId,
            result.ListingId,
            result.PaymentId,
            result.BuyerId,
            result.SellerId,
            result.ProductValue,
            result.CreatedAt,
            result.UpdatedAt,
            result.Status,
            result.DeliveryStatus,
            result.Dispute);
    }
}