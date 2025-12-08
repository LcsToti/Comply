using DnsClient.Internal;
using ListingService.App.Common;
using ListingService.App.Common.Interfaces;
using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.ListingService.Notifications.Purchase;
using Shared.Contracts.Messages.ListingService.Payments.Purchase;
using Shared.Contracts.Messages.ListingService.Sales;

namespace ListingService.App.Commands.ListingCommands.CompleteBuyNow;

public class CompleteBuyNowCommandHandler(
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
    IMessageBus messageBus,
    IDateTimeProvider dateTimeProvider,
    ILogger<CompleteBuyNowCommandHandler> logger) : IRequestHandler<CompleteBuyNowCommand>
{
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly ILogger<CompleteBuyNowCommandHandler> _logger = logger;

    public async Task Handle(CompleteBuyNowCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CompleteBuyNowCommand for Listing {ListingId}", request.ListingId);

        var utcNow = _dateTimeProvider.UtcNow;

        if (utcNow > request.ExpiresAt)
        {
            _logger.LogError(
                "CompleteBuyNowCommand for Listing {Id} has expired at {Expire}. Current time is {Now}. Refund will be requested.",
                request.ListingId,
                request.ExpiresAt,
                utcNow);

            var reason = "Buy now message's expiration time exceeded.";
            var refundMessage = new PurchaseRefundRequestMessage(
                AmountToRefund: request.Value,
                PaymentId: request.PaymentId,
                Reason: reason,
                UserId: request.BuyerId);

            await _messageBus.PublishAsync(refundMessage, cancellationToken);
            _logger.LogInformation("Refund request has been published. Reason: {Reason}", reason);
            return;
        }
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);
        if (listing is null)
        {
            _logger.LogError("Error while purchasing Listing {Id}. Listing not found.", request.ListingId);

            var reason = "Listing not found.";
            var refundMessage = new PurchaseRefundRequestMessage(
                AmountToRefund: request.Value,
                PaymentId: request.PaymentId,
                Reason: reason,
                UserId: request.BuyerId);

            await _messageBus.PublishAsync(refundMessage, cancellationToken);
            _logger.LogInformation("Refund request has been published. Reason: {Reason}", reason);
            return;
        }

        // Domain
        try
        {
            listing.CompleteBuyNow(request.BuyerId, utcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while buying listing {Id}", request.ListingId);

            var reason = ex.Message;
            var refundMessage = new PurchaseRefundRequestMessage(
                AmountToRefund: request.Value,
                PaymentId: request.PaymentId,
                Reason: reason,
                UserId: request.BuyerId);

            await _messageBus.PublishAsync(refundMessage, cancellationToken);
            _logger.LogInformation("Refund request has been published. Reason: {Reason}", reason);
            return;
        }

        if (listing.AuctionId is not null)
        {
            listing.DetachAuction(utcNow);
        }

        // Persist
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);

        // Messaging
        var createSaleMessage = new CreateSaleMessage(
            ProductId: listing.ProductId,
            BuyerId: request.BuyerId,
            SellerId: listing.SellerId,
            ListingId: listing.Id,
            PaymentId: request.PaymentId,
            ProductValue: request.Value);

        await _messageBus.PublishAsync(createSaleMessage, cancellationToken);
        _logger.LogInformation("CreateSaleMessage for Listing {Id} has been published.", listing.Id);

        // Notification Message
        var productBoughtNotificationMessage = new ProductBoughtNotificationMessage(
            ProductId: listing.ProductId,
            SellerId: listing.SellerId,
            BuyerId: request.BuyerId,
            Price: request.Value,
            BoughtAt: utcNow);
        await _messageBus.PublishAsync(productBoughtNotificationMessage, cancellationToken);
        _logger.LogInformation("ProductBoughtNotificationMessage for Listing {Id} has been published.", listing.Id);

        _logger.LogInformation("Buy Now purchase completed for listing {Id}", request.ListingId);
    }
}

