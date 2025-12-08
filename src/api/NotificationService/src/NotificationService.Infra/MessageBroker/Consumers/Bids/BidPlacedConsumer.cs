using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.ListingService.Notifications.Bid;

namespace NotificationService.Infra.MessageBroker.Consumers.Bids;

public class BidPlacedConsumer : IConsumer<BidPlacedNotificationMessage>
{
    private readonly ILogger<BidPlacedConsumer> _logger;
    private readonly IMediator _mediator;
    private readonly IWatchListRepository _watchListRepository;
    
    public BidPlacedConsumer(ILogger<BidPlacedConsumer> logger, IMediator mediator, IWatchListRepository watchListRepository)
    {
        _logger = logger;
        _mediator = mediator;
        _watchListRepository = watchListRepository;
    }
    
    public async Task Consume(ConsumeContext<BidPlacedNotificationMessage> context)
    {
        var msg = context.Message;
        
        #region Watchers

        var watchersMessage = $"Um novo lance foi feito! Dá seu lance logoo, ninguém vai te esperar! Já se deu por vencido?";
        
        var watcherUserIds = await _watchListRepository.GetUsersWatchingProductAsync(msg.ProductId);
        var userIds = watcherUserIds.ToList();
        
        foreach (var userId in userIds)
        {
            await _mediator.Send(new ProcessNotificationEvent(
                NotificationType.Auction,
                userId,
                watchersMessage,
                msg.ProductId
            ));
        }

        #endregion
        #region Seller
        
        var sellerMessage = $"Um novo lance foi feito em seu produto! Atualmente está em R${msg.NewBidValue}";
        
        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.SellerId,
            sellerMessage,
            msg.ProductId
        ));

        #endregion
        #region Bidder

        var bidderMessage = $"Parabéns pelo lance!! Você é o primeiro da fila.. mas como já dizia o grande homem: Os últimos serão os primeiros!";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.BidderId,
            bidderMessage,
            msg.ProductId
        ));
        
        #endregion

        _logger.LogInformation("Notifications sent for Bid Placed — Seller + Bidder + {Count} watchers", userIds.Count);
    }

}