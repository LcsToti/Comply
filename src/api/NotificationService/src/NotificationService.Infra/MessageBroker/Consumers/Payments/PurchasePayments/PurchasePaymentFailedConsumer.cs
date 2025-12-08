using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.PaymentsService.Purchase;

namespace NotificationService.Infra.MessageBroker.Consumers.Payments.PurchasePayments;

public class PurchasePaymentFailedConsumer : IConsumer<PurchasePaymentFailedMessage>
{
    private readonly ILogger<PurchasePaymentFailedConsumer> _logger;
    private readonly IMediator _mediator;
    
    public PurchasePaymentFailedConsumer(ILogger<PurchasePaymentFailedConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<PurchasePaymentFailedMessage> context)
    {
        var msg = context.Message;

        const string message = "Houve um problema ao realizar o pagamento.. Venha ver mais detalhes";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Payment,
            msg.BuyerId,
            message,
            msg.ListingId
        ));
        _logger.LogInformation("Notification Sent -- Purchase Payment Failed");
    }
}