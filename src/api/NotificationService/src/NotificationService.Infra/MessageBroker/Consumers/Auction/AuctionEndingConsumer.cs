using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.ListingService.Notifications.Auction;

namespace NotificationService.Infra.MessageBroker.Consumers.Auction;

public class AuctionEndingConsumer : IConsumer<AuctionEndingNotificationMessage>
{
    private readonly ILogger<AuctionEndingConsumer> _logger;
    private readonly IMediator _mediator;
    private readonly IWatchListRepository _watchListRepository;

    public AuctionEndingConsumer(ILogger<AuctionEndingConsumer> logger, IMediator mediator, IWatchListRepository watchListRepository)
    {
        _logger = logger;
        _mediator = mediator;
        _watchListRepository = watchListRepository;
    }

    public async Task Consume(ConsumeContext<AuctionEndingNotificationMessage> context)
    {
        var msg = context.Message;

        #region Seller

        var sellerMessage = $"Seu leilão está acabando! No momento estamos com {msg.BidCount} Lances!!!";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.SellerId,
            sellerMessage,
            msg.ProductId
        ));

        #endregion
        #region LastBidder
        if (msg.LastBidderId is not null)
        {
            var lastBidderMessage = $"O Leilão está terminando! Será que é seu?? Xiiiiii sei não heinnn";

            await _mediator.Send(new ProcessNotificationEvent(
                NotificationType.Auction,
                msg.LastBidderId.Value,
                lastBidderMessage,
                msg.ProductId
            ));
        }
        #endregion
        #region Watchers

        var watchersMessage = $"O Leilão está terminando! Cuidado para ninguém te ultrapassar!!";

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

        _logger.LogInformation("Notifications sent for Auction Ending — Seller + LastBidder + {Count} watchers", userIds.Count);
    }

}