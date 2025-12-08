using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.ListingService.Notifications.Bid;

namespace NotificationService.Infra.MessageBroker.Consumers.Bids;

public class BidOutbiddedConsumer : IConsumer<BidOutbiddedNotificationMessage>
{
    private readonly ILogger<BidOutbiddedConsumer> _logger;
    private readonly IMediator _mediator;
    
    public BidOutbiddedConsumer(ILogger<BidOutbiddedConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<BidOutbiddedNotificationMessage> context)
    {
        var msg = context.Message;
        
        #region Seller
        
        var sellerMessage = $"Ultrapassaram o lance atuaal!! Atualmente seu produto está em R${msg.NewBidValue}!!";
        
        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.SellerId,
            sellerMessage,
            msg.ProductId
        ));

        #endregion
        #region LastBidder
        
        var lastBidderMessage = $"Venho lhe informar a perda de alguém querido.... seu lance... foi.... ultrapassado";
        
        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.LastBidderId,
            lastBidderMessage,
            msg.ProductId
        ));
        
        #endregion
        #region NewBidder

        var newBidderMessage = $"FOGO NELES!!! Seu lance é vitorioso..(por enquanto).";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.NewBidderId,
            newBidderMessage,
            msg.ProductId
        ));
        
        #endregion

        _logger.LogInformation("Notifications sent for BidOutbidded  — Seller + LastBidder + NewBidder");
    }

}