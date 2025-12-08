using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.ListingService.Notifications.Auction;
using Shared.Contracts.Messages.PaymentsService.Bid;

namespace NotificationService.Infra.MessageBroker.Consumers.Auction;

public class AuctionEndedConsumer : IConsumer<AuctionEndedNotificationMessage>
{
    private readonly ILogger<AuctionEndedConsumer> _logger;
    private readonly IMediator _mediator;
    private readonly IWatchListRepository _watchListRepository;
    
    public AuctionEndedConsumer(ILogger<AuctionEndedConsumer> logger, IMediator mediator, IWatchListRepository watchListRepository)
    {
        _logger = logger;
        _mediator = mediator;
        _watchListRepository = watchListRepository;
    }
    
    public async Task Consume(ConsumeContext<AuctionEndedNotificationMessage> context)
    {
        var msg = context.Message;
        
        #region Watchers

        var watchersMessage = $"O Leilão acabou... foram {msg.BidCount} lances!";
        
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
        
        var sellerMessage = $"Seu leilão acabou com {msg.BidCount} lances e um valor total de R${msg.FinalValue} Parabéns!!!";
        
        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.SellerId,
            sellerMessage,
            msg.ProductId
        ));

        #endregion
        #region Winner

        var bidderMessage = $"É seuu!! Vamos ver como vamos buscar o item agora..";

        if (msg.WinnerId != null)
        {
            await _mediator.Send(new ProcessNotificationEvent(
                NotificationType.Auction,
                msg.WinnerId.Value,
                bidderMessage,
                msg.ProductId
            ));
        }
        
        
        #endregion

        _logger.LogInformation("Notifications sent for Auction Ended — Seller + Winner + {Count} watchers", userIds.Count);
    }

}