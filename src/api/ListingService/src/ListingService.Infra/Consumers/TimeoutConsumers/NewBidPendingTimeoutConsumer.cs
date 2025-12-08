using ListingService.App.Commands.AuctionCommands.AbortNewBid;
using ListingService.App.Messages.PaymentTimeoutMessages;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.Infra.Consumers.TimeoutConsumers;

public class NewBidPendingTimeoutConsumer(
    ISender sender,
    ILogger<NewBidPendingTimeoutConsumer> logger) : IConsumer<NewBidPendingTimeoutMessage>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<NewBidPendingTimeoutConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<NewBidPendingTimeoutMessage> context)
    {
        var msg = context.Message;

        _logger.LogInformation("Consuming NewBidPendingTimeoutMessage for AuctionId: {AuctionId}", context.Message.AuctionId);

        var command = new AbortNewBidCommand(AuctionId: msg.AuctionId);

        await _sender.Send(command);

        _logger.LogInformation("Processed NewBidPendingTimeoutMessage for AuctionId: {AuctionId}", msg.AuctionId);
    }
}
