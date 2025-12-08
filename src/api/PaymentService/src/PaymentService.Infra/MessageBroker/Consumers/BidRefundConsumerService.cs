using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.UseCases.RefundCases.CreateRefund;
using Payments.Infra.MessageBroker.Publishers;
using Shared.Contracts.Messages.ListingService.Payments.Bid;

namespace Payments.Infra.MessageBroker.Consumers;

public class BidRefundConsumerService : IConsumer<BidRefundRequestMessage>
{
    private readonly ILogger<BidRefundConsumerService> _logger;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;

    public BidRefundConsumerService(ILogger<BidRefundConsumerService> logger, IMediator mediator, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;       
    }

    public async Task Consume(ConsumeContext<BidRefundRequestMessage> context)
    {
        var msg = context.Message;
        var publisher = new BidPaymentResultPublisher(_publishEndpoint);
        
        _logger.LogInformation("Criando reembolso para o pagamento: {PaymentId} ", msg.PaymentId);
        await _mediator.Send(new CreateRefundEvent(msg.AmountToRefund, msg.PaymentId, msg.Reason, msg.UserId, publisher));
    }
}