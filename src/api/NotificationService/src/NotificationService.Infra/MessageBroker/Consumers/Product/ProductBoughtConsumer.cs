using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.ListingService.Notifications.Purchase;

namespace NotificationService.Infra.MessageBroker.Consumers.Product;

public class ProductBoughtConsumer : IConsumer<ProductBoughtNotificationMessage>
{
    private readonly ILogger<ProductBoughtConsumer> _logger;
    private readonly IMediator _mediator;
    private readonly IWatchListRepository _watchListRepository;
    
    public ProductBoughtConsumer(ILogger<ProductBoughtConsumer> logger, IMediator mediator, IWatchListRepository watchListRepository)
    {
        _logger = logger;
        _mediator = mediator;
        _watchListRepository = watchListRepository;
    }
    
    public async Task Consume(ConsumeContext<ProductBoughtNotificationMessage> context)
    {
        var msg = context.Message;
        
        #region Watchers

        var watchersMessage = $"";
        
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
        
        var sellerMessage = $"Seu produto foi comprado! Vai enviar ele antes que sobre pra gente!! Deu R${msg.Price}";
        
        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.SellerId,
            sellerMessage,
            msg.ProductId
        ));

        #endregion
        #region Buyer

        var buyerMessage = $"O produto é seu!! Aguardando delivery agora..";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Auction,
            msg.BuyerId,
            buyerMessage,
            msg.ProductId
        ));
        
        #endregion

        _logger.LogInformation("Notifications sent for Product Bought — Seller + Buyer + {Count} watchers", userIds.Count);
    }

}