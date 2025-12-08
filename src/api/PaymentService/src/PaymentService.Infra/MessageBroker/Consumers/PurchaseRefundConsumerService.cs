using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.UseCases.RefundCases.CreateRefund;
using Payments.Infra.MessageBroker.Publishers;
using Shared.Contracts.Messages.ListingService.Payments.Purchase;

namespace Payments.Infra.MessageBroker.Consumers;

public class PurchaseRefundConsumerService : IConsumer<PurchaseRefundRequestMessage>
{
    private readonly ILogger<PurchaseRefundConsumerService> _logger;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;   

    public PurchaseRefundConsumerService(ILogger<PurchaseRefundConsumerService> logger, IMediator mediator, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;       
    }

    public async Task Consume(ConsumeContext<PurchaseRefundRequestMessage> context)
    {
        var msg = context.Message;
        var publisher = new PurchasePaymentResultPublisher(_publishEndpoint);
        
        _logger.LogInformation("Criando reembolso para o pagamento: {PaymentId} ", msg.PaymentId);
        await _mediator.Send(new CreateRefundEvent(msg.AmountToRefund, msg.PaymentId, msg.Reason, msg.UserId, publisher));
    }
}