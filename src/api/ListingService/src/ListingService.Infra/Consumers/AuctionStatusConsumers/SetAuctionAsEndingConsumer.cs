using ListingService.App.Commands.AuctionCommands.AuctionStatusActions.SetAuctionAsEnding;
using ListingService.App.Messages.AuctionStateMessages;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.Infra.Consumers.AuctionStatusConsumers;

public class SetAuctionAsEndingConsumer(
    ISender _sender,
    ILogger<SetAuctionAsEndingConsumer> logger) : IConsumer<SetAuctionAsEndingMessage>
{
    private readonly ISender _sender = _sender;
    private readonly ILogger<SetAuctionAsEndingConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<SetAuctionAsEndingMessage> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Consuming SetAuctionAsEndingMessage for AuctionId: {AuctionId}, Version: {Version}",
            msg.AuctionId,
            msg.Version);

        var command = new SetAuctionAsEndingCommand(
            AuctionId: msg.AuctionId,
            Version: msg.Version);

        await _sender.Send(command, context.CancellationToken);

        _logger.LogInformation("SetAuctionAsEndingCommand sent for AuctionId: {AuctionId}", msg.AuctionId);
    }
}
