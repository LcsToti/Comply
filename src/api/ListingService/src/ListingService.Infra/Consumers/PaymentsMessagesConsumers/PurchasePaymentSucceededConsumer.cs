using ListingService.App.Commands.ListingCommands.CompleteBuyNow;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.PaymentsService.Purchase;

namespace ListingService.Infra.Consumers.PaymentsMessagesConsumers;

public class PurchasePaymentSucceededConsumer(
    ISender sender,
    ILogger<PurchasePaymentSucceededConsumer> logger) : IConsumer<PurchasePaymentSucceededMessage>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<PurchasePaymentSucceededConsumer> _logger = logger;
    public async Task Consume(ConsumeContext<PurchasePaymentSucceededMessage> context)
    {
        var msg = context.Message;
       
        _logger.LogInformation(
            "Consuming PurchasePaymentSucceededMessage for ListingId: {ListingId}, BuyerId: {BuyerId}",
            msg.ListingId,
            msg.BuyerId);

        var command = new CompleteBuyNowCommand(
            ListingId: msg.ListingId,
            BuyerId: msg.BuyerId,
            ExpiresAt: msg.ExpiresAt,
            PaymentId: msg.PaymentId,
            Value: msg.Price);

        await _sender.Send(command, context.CancellationToken);

        _logger.LogInformation("CompleteBuyNowCommand sent for ListingId: {ListingId}", msg.ListingId);
    }
}