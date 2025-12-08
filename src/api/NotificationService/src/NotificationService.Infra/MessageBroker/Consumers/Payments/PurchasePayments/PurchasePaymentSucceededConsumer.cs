using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.PaymentsService.Purchase;

namespace NotificationService.Infra.MessageBroker.Consumers.Payments.PurchasePayments;

public class PurchasePaymentSucceededConsumer : IConsumer<PurchasePaymentSucceededMessage>
{
    private readonly ILogger<PurchasePaymentSucceededConsumer> _logger;
    private readonly IMediator _mediator;
    
    public PurchasePaymentSucceededConsumer(ILogger<PurchasePaymentSucceededConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<PurchasePaymentSucceededMessage> context)
    {
        var msg = context.Message;

        var message = $"Sua compra foi aprovada no valor de R${msg.Price}.. Venha dar uma olhada";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Payment,
            msg.BuyerId,
            message,
            msg.ListingId
        ));
        _logger.LogInformation("Notification Sent -- Purchase Payment Succeeded");
    }
}