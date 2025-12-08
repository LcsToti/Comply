using ListingService.App.Common;
using ListingService.App.Common.Interfaces;
using ListingService.Domain.AuctionAggregate.Enums;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.ListingService.Notifications.Auction;
using Shared.Contracts.Messages.ListingService.Sales;

namespace ListingService.App.Commands.AuctionCommands.AuctionStatusActions.FinishAuction;

public class FinishAuctionCommandHandler(
    IAuctionRepository auctionRepository,
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator orchestrator,
    IMessageBus messageBus,
    IDateTimeProvider dateTimeProvider,
    ILogger<FinishAuctionCommandHandler> logger) : IRequestHandler<FinishAuctionCommand>
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _orchestrator = orchestrator;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<FinishAuctionCommandHandler> _logger = logger;

    public async Task Handle(FinishAuctionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling FinishAuctionCommand for Auction {AuctionId}", request.AuctionId);

        var now = _dateTimeProvider.UtcNow;

        // Validations
        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId);
        if (auction is null)
        {
            _logger.LogError("Error while consuming FinishAuctionCommand: Auction {AuctionId} not found.", request.AuctionId);
            return;
        }

        var listing = await _listingRepository.GetByIdAsync(auction.ListingId);
        if (listing is null)
        {
            _logger.LogError("Error while consuming FinishAuctionCommand: Listing {ListingId} not found.", auction.ListingId);
            return;
        }

        if (auction.Version != request.Version)
        {
            _logger.LogError(
                "Fail while consuming FinishAuctionCommand: Auction version is {AuctionVersion} and Message Version Is {MessageVersion}.", 
                auction.Version, 
                request.Version);
            return;
        }

        // Domain
        auction.Finish(now);

        // Domain Effects
        if (auction.Status == AuctionStatus.Success)
        {
            var winningBid = auction.Bids.FirstOrDefault(b => b.Status == BidStatus.Winner);

            listing.MarkAsSoldByAuction(winningBid!.BidderId, now);
            listing.SetAuctionInactive(now);

            // Messaging - Create Sale
            var createSaleMessage = new CreateSaleMessage(
            ProductId: listing.ProductId,
            BuyerId: winningBid.BidderId,
            SellerId: listing.SellerId,
            ListingId: listing.Id,
            PaymentId: winningBid.PaymentId,
            ProductValue: winningBid.Value);

            await _messageBus.PublishAsync(createSaleMessage, cancellationToken);
            _logger.LogInformation("CreateSaleMessage for Auction {AuctionId} has been published.", auction.Id);

            // Notification Message
            var auctionEndedNotificationMessage = new AuctionEndedNotificationMessage(
                ProductId: listing.ProductId,
                SellerId: listing.SellerId,
                BidCount: auction.Bids.Count,
                Status: auction.Status.ToString(),
                WinnerId: winningBid.BidderId,
                FinalValue: winningBid.Value);
            await _messageBus.PublishAsync(auctionEndedNotificationMessage, cancellationToken);
            _logger.LogInformation("AuctionEndedNotificationMessage for Auction {AuctionId} has been published.", auction.Id);
        }
        else
        {
            listing.SetAuctionInactive(now);
            listing.DetachAuction(now);
        }

        // Persist
        await _orchestrator.UpdateAuctionAsync(auction, cancellationToken);
        await _orchestrator.UpdateListingAsync(listing, cancellationToken);
        
        _logger.LogInformation("Auction {AuctionId} finished.", auction.Id);
    }
}