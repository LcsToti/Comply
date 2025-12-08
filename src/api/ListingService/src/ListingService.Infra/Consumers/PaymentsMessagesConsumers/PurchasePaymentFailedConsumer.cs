using ListingService.App.Commands.ListingCommands.AbortBuyNow;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.PaymentsService.Purchase;

namespace ListingService.Infra.Consumers.PaymentsMessagesConsumers;

public class PurchasePaymentFailedConsumer(
    ISender sender,
    ILogger<PurchasePaymentFailedConsumer> logger) : IConsumer<PurchasePaymentFailedMessage>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<PurchasePaymentFailedConsumer> _logger = logger;
    public async Task Consume(ConsumeContext<PurchasePaymentFailedMessage> context)
    {
        var msg = context.Message;

        _logger.LogInformation("Consuming PurchasePaymentFailedMessage for ListingId: {ListingId}", msg.ListingId);

        var command = new AbortBuyNowCommand(
            ListingId: msg.ListingId,
            ExpiresAt: msg.ExpiresAt);

        await _sender.Send(command, context.CancellationToken);

        _logger.LogInformation("AbortBuyNowCommand sent for ListingId: {ListingId}", msg.ListingId);
    }
}