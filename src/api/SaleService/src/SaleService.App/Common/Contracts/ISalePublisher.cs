namespace SalesService.App.Common.Contracts;

public interface ISalePublisher
{
    Task PublishSaleRefundAsync(Guid paymentId , decimal value, string reason, Guid userId);
    Task PublishSaleDeliveredAsync(Guid paymentId, Guid sellerId);
    Task PublishDisputeExpirationDateAsync(Guid saleId, TimeSpan expiresAt);
}