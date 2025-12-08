using ListingService.App.Commands.AuctionCommands.CompleteNewBid;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.PaymentsService.Bid;

namespace ListingService.Infra.Consumers.PaymentsMessagesConsumers;

public class BidPaymentSucceededConsumer(
    ISender sender,
    ILogger<BidPaymentSucceededConsumer> logger) : IConsumer<BidPaymentSucceededMessage>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<BidPaymentSucceededConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<BidPaymentSucceededMessage> context)
    {
        var msg = context.Message;
       
        _logger.LogInformation(
            "Consuming BidPaymentSucceededMessage for AuctionId: {AuctionId}, BidderId: {BidId}, BidValue: {BidValue}",
            msg.AuctionId,
            msg.BidderId,
            msg.BidValue);

        var command = new CompleteNewBidCommand(
            AuctionId: msg.AuctionId,
            BidderId: msg.BidderId,
            BidValue: msg.BidValue,
            ExpiresAt: msg.ExpiresAt,
            PaymentId: msg.PaymentId);

        await _sender.Send(command);

        _logger.LogInformation("CompleteNewBidCommand sent for AuctionId: {AuctionId}", msg.AuctionId);
    }
}