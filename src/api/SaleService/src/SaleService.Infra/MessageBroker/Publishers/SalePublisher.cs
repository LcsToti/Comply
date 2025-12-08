using MassTransit;
using SalesService.App.Common.Contracts;
using SalesService.App.Events.SaleEvents.CloseDisputeAsExpired;
using Shared.Contracts.Messages.PaymentsService;

namespace SalesService.Infra.MessageBroker.Publishers;

public class SalePublisher : ISalePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    public SalePublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task PublishSaleRefundAsync(Guid paymentId, decimal value, string reason, Guid userId)
    {
        await _publishEndpoint.Publish(
            new ApprovedToRefundMessage(paymentId, value, reason, userId)
        );
    }

    public async Task PublishSaleDeliveredAsync(Guid paymentId, Guid sellerId)
    {
        await _publishEndpoint.Publish(
            new ApproveWithdrawalMessage(paymentId, sellerId)
        );
    }

    public async Task PublishDisputeExpirationDateAsync(Guid saleId, TimeSpan expiresAt)
    {
        await _publishEndpoint.Publish( new CloseDisputeAsExpiredEvent(saleId, expiresAt), ctx =>
        {
            ctx.Delay = expiresAt;
        });
    }
}