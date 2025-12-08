using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.PaymentsService.Bid;

namespace NotificationService.Infra.MessageBroker.Consumers.Payments.BidPayments;

public class BidPaymentRefundedConsumer : IConsumer<BidPaymentRefundedMessage>
{
    private readonly ILogger<BidPaymentRefundedConsumer> _logger;
    private readonly IMediator _mediator;
    
    public BidPaymentRefundedConsumer(ILogger<BidPaymentRefundedConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<BidPaymentRefundedMessage> context)
    {
        var msg = context.Message;

        const string message = "Seu pagamento foi reembolsado.. Venha ver mais detalhes";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Payment,
            msg.BidderId,
            message,
            msg.AuctionId
        ));
        _logger.LogInformation("Notification Sent -- Bid Payment Refunded");
    }
}