using MassTransit;
using Payments.App.Common.Contracts;
using Shared.Contracts.Messages.ListingService.Payments.Bid;
using Shared.Contracts.Messages.PaymentsService.Bid;

namespace Payments.Infra.MessageBroker.Publishers;

public class BidPaymentResultPublisher : IPaymentPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public BidPaymentResultPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishPaymentSucceededAsync(Guid sourceId, Guid buyerId, DateTime expiresAt, decimal value, Guid paymentId, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new BidPaymentSucceededMessage(sourceId, buyerId, value, expiresAt, paymentId), 
            cancellationToken
        );
    }

    public async Task PublishPaymentFailedAsync(Guid sourceId, Guid buyerId, DateTime expiresAt, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(
            new BidPaymentFailedMessage(sourceId, buyerId, expiresAt), 
            cancellationToken
        );
    }
}