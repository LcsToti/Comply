using ListingService.App.Commands.AuctionCommands.AuctionStatusActions.StartAuction;
using ListingService.App.Messages.AuctionStateMessages;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.Infra.Consumers.AuctionStatusConsumers;

public class StartAuctionConsumer(
    ISender _sender,
    ILogger<StartAuctionConsumer> logger) : IConsumer<StartAuctionMessage>
{
    private readonly ISender _sender = _sender;
    private readonly ILogger<StartAuctionConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<StartAuctionMessage> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Consuming StartAuctionMessage for AuctionId: {AuctionId}, Version: {Version}",
            msg.AuctionId,
            msg.Version);

        var command = new StartAuctionCommand(
            AuctionId: msg.AuctionId,
            Version: msg.Version);

        await _sender.Send(command, context.CancellationToken);

        _logger.LogInformation("StartAuctionCommand sent for AuctionId: {AuctionId}", msg.AuctionId);
    }
}

