using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.ListingService.Notifications.Auction;

namespace NotificationService.Infra.MessageBroker.Consumers.Auction;

public class AuctionExtendedConsumer : IConsumer<AuctionExtendedNotificationMessage>
{
    private readonly ILogger<AuctionExtendedConsumer> _logger;
    private readonly IMediator _mediator;
    private readonly IWatchListRepository _watchListRepository;

    public AuctionExtendedConsumer(ILogger<AuctionExtendedConsumer> logger, IMediator mediator, IWatchListRepository watchListRepository)
    {
        _logger = logger;
        _mediator = mediator;
        _watchListRepository = watchListRepository;
    }

    public async Task Consume(ConsumeContext<AuctionExtendedNotificationMessage> context)
    {
        var msg = context.Message;

        var remaining = msg.NewEndDate - DateTime.UtcNow;

        string remainingText;
        if (remaining.TotalHours >= 1)
            remainingText = $"{(int)remaining.TotalHours}h {remaining.Minutes}min";
        else if (remaining.TotalMinutes >= 1)
            remainingText = $"{(int)remaining.TotalMinutes}min {remaining.Seconds}s";
        else
            remainingText = $"{remaining.Seconds}s";


        #region Watchers

        var watchersMessage = $"O Leilão foi extendido!! Tá pegando fogo bixo! Já foram {msg.BidCount} lances! Encerra em {remainingText}";

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

        var sellerMessage = $"Seu leilão foi extendido! Que produtão hein! Encerrará em {remainingText}";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.SellerId,
            sellerMessage,
            msg.ProductId
        ));

        #endregion
        #region Last Bidder
        if (msg.LastBidderId is not null)
        {
            var lastBidderMessage = $"Seu lance foi ultrapassado KKKKKK Tá na hora do showwwww! Leilão extendido! Encerra em {remainingText}";

            await _mediator.Send(new ProcessNotificationEvent(
                NotificationType.Auction,
                msg.LastBidderId.Value,
                lastBidderMessage,
                msg.ProductId
            ));
        }
        #endregion
        #region Bidder

        var bidderMessage = $"Amostradinho!! Agora o lance foi extendido! Encerra em {remainingText}";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.BidderId,
            bidderMessage,
            msg.ProductId
        ));

        #endregion

        _logger.LogInformation("Notifications sent for Auction Extended — Seller + LastBidder + Bidder + {Count} watchers", userIds.Count);
    }

}