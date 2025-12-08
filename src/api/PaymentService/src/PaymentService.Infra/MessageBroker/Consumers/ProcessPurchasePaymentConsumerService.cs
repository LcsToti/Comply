using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.UseCases.PaymentCases.CreatePayment;
using Payments.Infra.MessageBroker.Publishers;
using Shared.Contracts.Messages.ListingService.Payments.Purchase;

namespace Payments.Infra.MessageBroker.Consumers;

public class ProcessPurchasePaymentConsumerService : IConsumer<PurchasePaymentRequestMessage>
{
    private readonly ILogger<ProcessPurchasePaymentConsumerService> _logger;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public ProcessPurchasePaymentConsumerService(ILogger<ProcessPurchasePaymentConsumerService> logger, IMediator mediator, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<PurchasePaymentRequestMessage> context)
    {
        var msg = context.Message;
        var publisher = new PurchasePaymentResultPublisher(_publishEndpoint);
        
        _logger.LogInformation("Começando o processo de pagamento para o produto: {ListingId}", msg.ListingId);
        await _mediator.Send(new CreatePaymentEvent(msg.ListingId, msg.BuyerId, msg.SellerId, msg.ExpiresAt, msg.Value,
            msg.PaymentMethod, publisher));
    }
}