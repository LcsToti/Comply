using ListingService.App.Commands.AuctionCommands.AbortNewBid;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.PaymentsService.Bid;

namespace ListingService.Infra.Consumers.PaymentsMessagesConsumers;

public class BidPaymentFailedConsumer(
    ISender sender,
    ILogger<BidPaymentFailedConsumer> logger) : IConsumer<BidPaymentFailedMessage>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<BidPaymentFailedConsumer> _logger = logger;
    public async Task Consume(ConsumeContext<BidPaymentFailedMessage> context)
    {
        var msg = context.Message;
        
        _logger.LogInformation(
            "Consuming BidPaymentFailedMessage for AuctionId: {AuctionId}",
            msg.AuctionId);

        var command = new AbortNewBidCommand(
            AuctionId: msg.AuctionId,
            ExpiresAt: msg.ExpiresAt);

        await _sender.Send(command);

        _logger.LogInformation("AbortNewBidCommand sent for AuctionId: {AuctionId}", msg.AuctionId);
    }
}