using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.ListingService.Notifications.Auction;
using Shared.Contracts.Messages.PaymentsService.Bid;

namespace NotificationService.Infra.MessageBroker.Consumers.Auction;

public class AuctionStartConsumer : IConsumer<AuctionStartedNotificationMessage>
{
    private readonly ILogger<AuctionStartConsumer> _logger;
    private readonly IMediator _mediator;
    private readonly IWatchListRepository _watchListRepository;
    
    public AuctionStartConsumer(ILogger<AuctionStartConsumer> logger, IMediator mediator, IWatchListRepository watchListRepository)
    {
        _logger = logger;
        _mediator = mediator;
        _watchListRepository = watchListRepository;
    }
    
    public async Task Consume(ConsumeContext<AuctionStartedNotificationMessage> context)
    {
        var msg = context.Message;

        #region Seller
        
        const string sellerMessage = "O seu leilão iniciou! Os lances vão começar!";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.SellerId,
            sellerMessage,
            msg.ProductId
        ));


        #endregion
        #region Watchers

        var watchersMessage = $"O leilão que você está acompanhando começou! Começa em R${msg.MinBidValue}!!";
        
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

        _logger.LogInformation("Notifications sent for Auction Started — Seller + {Count} watchers", userIds.Count);
    }

}