using ListingService.App.Commands.AuctionCommands.AuctionStatusActions.FinishAuction;
using ListingService.App.Messages.AuctionStateMessages;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.Infra.Consumers.AuctionStatusConsumers;

public class FinishAuctionConsumer(
    ISender sender,
    ILogger<FinishAuctionConsumer> logger) : IConsumer<FinishAuctionMessage>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<FinishAuctionConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<FinishAuctionMessage> context)
    {
        var msg = context.Message;

        _logger.LogInformation(
            "Consuming FinishAuctionMessage for AuctionId: {AuctionId}, Version: {Version}",
            msg.AuctionId,
            msg.Version);

        var command = new FinishAuctionCommand(
            AuctionId: msg.AuctionId,
            Version: msg.Version);

        await _sender.Send(command, context.CancellationToken);

        _logger.LogInformation("FinishAuctionCommand sent for AuctionId: {AuctionId}", msg.AuctionId);
    }
}
