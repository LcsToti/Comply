using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.UseCases.RefundCases.CreateRefund;
using Shared.Contracts.Messages.PaymentsService;

namespace Payments.Infra.MessageBroker.Consumers;

public class DisputeRefundConsumerService : IConsumer<ApprovedToRefundMessage>
{
    private readonly ILogger<DisputeRefundConsumerService> _logger;
    private readonly IMediator _mediator;

    public DisputeRefundConsumerService(ILogger<DisputeRefundConsumerService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;  
    }

    public async Task Consume(ConsumeContext<ApprovedToRefundMessage> context)
    {
        var msg = context.Message;
        
        _logger.LogInformation("Resolvendo disputa criando reembolso para o pagamento: {PaymentId} ", msg.PaymentId);
        await _mediator.Send(new CreateRefundEvent(msg.AmountToRefund, msg.PaymentId, msg.Reason, msg.UserId, null));
    }
}