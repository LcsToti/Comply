using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.PaymentsService.Purchase;

namespace NotificationService.Infra.MessageBroker.Consumers.Payments.PurchasePayments;

public class PurchasePaymentRefundedConsumer : IConsumer<PurchasePaymentRefundedMessage>
{
    private readonly ILogger<PurchasePaymentRefundedConsumer> _logger;
    private readonly IMediator _mediator;
    
    public PurchasePaymentRefundedConsumer(ILogger<PurchasePaymentRefundedConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<PurchasePaymentRefundedMessage> context)
    {
        var msg = context.Message;

        const string message = "Seu pagamento foi reembolsado.. Venha ver mais detalhes";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Payment,
            msg.BuyerId,
            message,
            msg.ListingId
        ));
        _logger.LogInformation("Notification Sent -- Purchase Payment Refunded");
    }
}