using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.PaymentsService.Bid;

namespace NotificationService.Infra.MessageBroker.Consumers.Payments.BidPayments;

public class BidPaymentFailedConsumer : IConsumer<BidPaymentFailedMessage>
{
    private readonly ILogger<BidPaymentFailedConsumer> _logger;
    private readonly IMediator _mediator;
    
    public BidPaymentFailedConsumer(ILogger<BidPaymentFailedConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<BidPaymentFailedMessage> context)
    {
        var msg = context.Message;

        const string message = "Houve um problema ao realizar o pagamento.";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Payment,
            msg.BidderId,
            message,
            msg.AuctionId
        ));
        _logger.LogInformation("Notification Sent -- Bid Payment Failed");
    }
}