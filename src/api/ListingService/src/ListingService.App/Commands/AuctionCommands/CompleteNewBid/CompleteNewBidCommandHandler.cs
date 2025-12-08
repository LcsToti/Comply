using ListingService.App.Common;
using ListingService.App.Common.Interfaces;
using ListingService.App.Messages.AuctionStateMessages;
using ListingService.Domain.AuctionAggregate.Enums;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.ListingService.Notifications.Auction;
using Shared.Contracts.Messages.ListingService.Notifications.Bid;
using Shared.Contracts.Messages.ListingService.Payments.Bid;
using Shared.Contracts.Messages.ListingService.Sales;

namespace ListingService.App.Commands.AuctionCommands.CompleteNewBid;

public class CompleteNewBidCommandHandler(
    IAuctionRepository auctionRepository,
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
    IDateTimeProvider dateTimeProvider,
    IMessageBus messageBus,
    ILogger<CompleteNewBidCommandHandler> logger) : IRequestHandler<CompleteNewBidCommand> 
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly ILogger<CompleteNewBidCommandHandler> _logger = logger;

    public async Task Handle(CompleteNewBidCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CompleteNewBidCommand for Auction {AuctionId}.", request.AuctionId);

        var utcNow = _dateTimeProvider.UtcNow;

        if (utcNow > request.ExpiresAt)
        {
            _logger.LogWarning("CompleteNewBidCommand for Auction {AuctionId} has expired at {ExpiresAt}. Current time is {UtcNow}. Refund will be requested.", request.AuctionId, request.ExpiresAt, utcNow);

            var reason = "Bid's message expiration time exceeded.";
            var refundMessage = new BidRefundRequestMessage(
                AmountToRefund: request.BidValue,
                PaymentId: request.PaymentId,
                Reason: reason,
                UserId: request.BidderId);

            await _messageBus.PublishAsync(refundMessage, cancellationToken);
            _logger.LogInformation("Refund request has been published. Reason: {Reason}", reason);
            return;
        }

        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId);
        if (auction is null)
        {
            _logger.LogError("Error while placing new bid on auction {Id}: Auction not found", request.AuctionId);

            var reason = "Bid's auction not found.";
            var refundMessage = new BidRefundRequestMessage(
                AmountToRefund: request.BidValue,
                PaymentId: request.PaymentId,
                Reason: reason,
                UserId: request.BidderId);

            await _messageBus.PublishAsync(refundMessage, cancellationToken);
            _logger.LogInformation("Refund request has been published. Reason: {Reason}", reason);
            return;
        }

        var listing = await _listingRepository.GetByIdAsync(auction.ListingId);
        if (listing is null)
        {
            _logger.LogError("Error while placing new bid on auction {Id}: Listing not found", auction.ListingId);

            var reason = "Bid's listing not found.";
            var refundMessage = new BidRefundRequestMessage(
                AmountToRefund: request.BidValue,
                PaymentId: request.PaymentId,
                Reason: reason,
                UserId: request.BidderId);

            await _messageBus.PublishAsync(refundMessage, cancellationToken);
            _logger.LogInformation("Refund request has been published. Reason: {Reason}", reason);
            return;
        }

        // Domain
        if (auction.Bids.Count != 0)
        {
            var lastWinningBid = auction.Bids.FirstOrDefault(b => b.Status == BidStatus.Winning);
            if (lastWinningBid is not null)
            {
                lastWinningBid.MarkAsOutbid(utcNow);

                var reason = "Outbid by another user.";
                var refundMessage = new BidRefundRequestMessage(
                   AmountToRefund: lastWinningBid.Value,
                   PaymentId: lastWinningBid.PaymentId,
                   Reason: reason,
                   UserId: request.BidderId);

                await _messageBus.PublishAsync(refundMessage, cancellationToken);
                _logger.LogInformation("Last winning bid refund request has been published. Reason: {Reason}", reason);

                var bidOutbiddedNotificationMessage = new BidOutbiddedNotificationMessage(
                    ProductId: listing.ProductId,
                    SellerId: listing.SellerId,
                    NewBidderId: request.BidderId,
                    LastBidderId: lastWinningBid.BidderId,
                    NewBidValue: request.BidValue);

                await _messageBus.PublishAsync(bidOutbiddedNotificationMessage, cancellationToken);
                _logger.LogInformation("BidOutbiddedNotificationMessage for Auction {AuctionId} has been published.", auction.Id);
            }
        }

        try
        {
            auction.CompleteNewBid(
                bidderId: request.BidderId, 
                bidValue: request.BidValue, 
                paymentId: request.PaymentId,
                utcNow: utcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while completing new bid on auction {Id}", request.AuctionId);

            var refundMessage = new BidRefundRequestMessage(
               AmountToRefund: request.BidValue,
               PaymentId: request.PaymentId,
               Reason: "Bid processing time exceeded.",
               UserId: request.BidderId);

            await _messageBus.PublishAsync(refundMessage, cancellationToken);
            _logger.LogInformation("Refund request for expired CompleteNewBidCommand for Auction {AuctionId} has been published.", request.AuctionId);
            return;
        }

        if (request.BidValue == auction.Settings.WinBidValue)
        {
            auction.Finish(utcNow);

            listing.MarkAsSoldByAuction(request.BidderId, utcNow);
            listing.SetAuctionInactive(utcNow);

            // Messaging - Create Sale
            var createSaleMessage = new CreateSaleMessage(
            ProductId: listing.ProductId,
            BuyerId: request.BidderId,
            SellerId: listing.SellerId,
            ListingId: listing.Id,
            PaymentId: request.PaymentId,
            ProductValue: request.BidValue);

            await _messageBus.PublishAsync(createSaleMessage, cancellationToken);
            _logger.LogInformation("CreateSaleMessage for Auction {AuctionId} has been published.", auction.Id);

            // Notification Message
            var auctionEndedNotificationMessage = new AuctionEndedNotificationMessage(
                ProductId: listing.ProductId,
                SellerId: listing.SellerId,
                BidCount: auction.Bids.Count,
                Status: auction.Status.ToString(),
                WinnerId: request.BidderId,
                FinalValue: request.BidValue);
            await _messageBus.PublishAsync(auctionEndedNotificationMessage, cancellationToken);
            _logger.LogInformation("AuctionEndedNotificationMessage for Auction {AuctionId} has been published.", auction.Id);
        }
        else
        {
            if (auction.Status == AuctionStatus.Ending)
            {
                auction.ExtendAuction();

                var delay = auction.Settings.EndDate - utcNow;
                var finishAuctionMessage = new FinishAuctionMessage(auction.Id, auction.Version);

                await _messageBus.PublishAsync(finishAuctionMessage, ctx => { ctx.Delay = delay; }, cancellationToken);
                _logger.LogInformation("Scheduled FinishAuctionMessage for Auction {AuctionId} with delay of {Delay}.", auction.Id, delay);

                // Notification Message
                var lastWinningBid = auction.Bids.FirstOrDefault(b => b.Status == BidStatus.Winning);

                var AuctionExtendedNotificationMessage = new AuctionExtendedNotificationMessage(
                    ProductId: listing.ProductId,
                    SellerId: listing.SellerId,
                    BidderId: request.BidderId,
                    LastBidderId: lastWinningBid?.BidderId,
                    BidCount: auction.Bids.Count,
                    NewEndDate: auction.Settings.EndDate);
                await _messageBus.PublishAsync(AuctionExtendedNotificationMessage, cancellationToken);
                _logger.LogInformation("AuctionExtendedNotificationMessage for Auction {AuctionId} has been published.", auction.Id);
            }
        }

        // Persist
        await _repositoryCommandsOrchestrator.UpdateAuctionAsync(auction, cancellationToken);
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);

        // Messaging

        // Notification Message
        var bidPlacedNotificationMessage = new BidPlacedNotificationMessage(
            ProductId: listing.ProductId,
            SellerId: listing.SellerId,
            BidderId: request.BidderId,
            NewBidValue: request.BidValue,
            PlacedAt: utcNow);
        await _messageBus.PublishAsync(bidPlacedNotificationMessage, cancellationToken);
        _logger.LogInformation("BidPlacedNotificationMessage for Auction {AuctionId} has been published.", auction.Id);

        _logger.LogInformation("CompleteNewBidCommand handled successfully for Auction {AuctionId}.", auction.Id);
    }
}

