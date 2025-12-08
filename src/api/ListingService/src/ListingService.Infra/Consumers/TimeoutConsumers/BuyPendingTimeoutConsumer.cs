using ListingService.App.Commands.ListingCommands.AbortBuyNow;
using ListingService.App.Messages.PaymentTimeoutMessages;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.Infra.Consumers.TimeoutConsumers;

public class BuyPendingTimeoutConsumer(
    ISender sender,
    ILogger<BuyPendingTimeoutConsumer> logger) : IConsumer<BuyPendingTimeoutMessage>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<BuyPendingTimeoutConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<BuyPendingTimeoutMessage> context)
    {
        var msg = context.Message;
        
        _logger.LogInformation("Consuming BuyPendingTimeoutMessage for ListingId: {ListingId}", msg.ListingId);

        var command = new AbortBuyNowCommand(ListingId: msg.ListingId);

        await _sender.Send(command, context.CancellationToken);

        _logger.LogInformation("AbortBuyNowCommand sent for ListingId: {ListingId}", msg.ListingId);
    }
}
