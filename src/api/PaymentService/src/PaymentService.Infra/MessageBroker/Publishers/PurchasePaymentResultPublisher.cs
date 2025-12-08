using MassTransit;
using Payments.App.Common.Contracts;
using Shared.Contracts.Messages.PaymentsService.Purchase;

namespace Payments.Infra.MessageBroker.Publishers;

public class PurchasePaymentResultPublisher : IPaymentPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PurchasePaymentResultPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishPaymentSucceededAsync(Guid sourceId, Guid buyerId, DateTime expiresAt, decimal value, Guid paymentId, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new PurchasePaymentSucceededMessage(
                ListingId: sourceId,
                BuyerId: buyerId,
                ExpiresAt: expiresAt,
                PaymentId: paymentId,
                Price: value), 
            cancellationToken);
    }

    public async Task PublishPaymentFailedAsync(Guid sourceId, Guid buyerId, DateTime expiresAt, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new PurchasePaymentFailedMessage(sourceId, buyerId, expiresAt), 
            cancellationToken
        );
    }
}