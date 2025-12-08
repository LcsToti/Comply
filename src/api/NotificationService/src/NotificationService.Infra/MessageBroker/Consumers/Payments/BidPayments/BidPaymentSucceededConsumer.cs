using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.PaymentsService.Bid;

namespace NotificationService.Infra.MessageBroker.Consumers.Payments.BidPayments;

public class BidPaymentSucceededConsumer : IConsumer<BidPaymentSucceededMessage>
{
    private readonly ILogger<BidPaymentSucceededConsumer> _logger;
    private readonly IMediator _mediator;
    
    public BidPaymentSucceededConsumer(ILogger<BidPaymentSucceededConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<BidPaymentSucceededMessage> context)
    {
        var msg = context.Message;

        var message = $"Seu lance foi efetuado com sucesso com o valor de R${msg.BidValue}";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Payment,
            msg.BidderId,
            message,
            msg.AuctionId
        ));
        _logger.LogInformation("Notification Sent -- Bid Payment Succeeded");
    }
}