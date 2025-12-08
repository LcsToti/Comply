using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.UseCases.PaymentCases.CreatePayment;
using Payments.Infra.MessageBroker.Publishers;
using Shared.Contracts.Messages.ListingService.Payments.Bid;

namespace Payments.Infra.MessageBroker.Consumers;

public class ProcessBidPaymentConsumerService : IConsumer<BidPaymentRequestMessage>
{
    private readonly ILogger<ProcessBidPaymentConsumerService> _logger;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public ProcessBidPaymentConsumerService(ILogger<ProcessBidPaymentConsumerService> logger, IMediator mediator, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;       
    }

    public async Task Consume(ConsumeContext<BidPaymentRequestMessage> context)
    {
        var msg = context.Message;

        var publisher = new BidPaymentResultPublisher(_publishEndpoint);

        _logger.LogInformation("Começando o processo de pagamento para o produto: {AuctionId}", msg.AuctionId);
        await _mediator.Send(new CreatePaymentEvent(msg.AuctionId, msg.BidderId, msg.SellerId, msg.ExpiresAt, msg.Value,
            msg.PaymentMethod, publisher));
    }
}