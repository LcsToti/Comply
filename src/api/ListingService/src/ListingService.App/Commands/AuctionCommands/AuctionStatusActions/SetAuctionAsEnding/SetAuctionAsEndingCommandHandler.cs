using ListingService.App.Common;
using ListingService.App.Common.Interfaces;
using ListingService.App.Messages.AuctionStateMessages;
using ListingService.Domain.AuctionAggregate.Enums;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.ListingService.Notifications.Auction;

namespace ListingService.App.Commands.AuctionCommands.AuctionStatusActions.SetAuctionAsEnding;

public class SetAuctionAsEndingCommandHandler(
    IAuctionRepository auctionRepository,
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator orchestrator,
    IDateTimeProvider dateTimeProvider,
    IMessageBus messageBus,
    ILogger<SetAuctionAsEndingCommandHandler> logger) : IRequestHandler<SetAuctionAsEndingCommand>
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _orchestrator = orchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly ILogger<SetAuctionAsEndingCommandHandler> _logger = logger;

    public async Task Handle(SetAuctionAsEndingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SetAuctionAsEndingCommand for Auction {AuctionId}.", request.AuctionId);

        // Validations
        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId);
        if (auction is null)
        {
            _logger.LogError("Error while consuming SetAuctionAsEndingCommand: Auction {AuctionId} not found.", request.AuctionId);
            return;
        }

        if (auction.Version != request.Version)
        {
            _logger.LogError(
                "Fail while consuming SetAuctionAsEndingCommand: Auction version is {AuctionVersion} and Message Version Is {MessageVersion}.", 
                auction.Version, 
                request.Version);
            return;
        }

        // Domain
        auction.SetAsEnding();

        // Persist
        await _orchestrator.UpdateAuctionAsync(auction, cancellationToken);

        // RabbitMQ
        var finishAuctionMessage = new FinishAuctionMessage(auction.Id, auction.Version);
        var delay = auction.Settings.EndDate - _dateTimeProvider.UtcNow;
        await _messageBus.PublishAsync(finishAuctionMessage, ctx => { ctx.Delay = delay; }, cancellationToken);
        _logger.LogInformation(
            "Scheduled FinishAuctionMessage for Auction {AuctionId} with delay of {Delay}.", 
            auction.Id, 
            delay);

        _logger.LogInformation("SetAuctionAsEndingMessage success: Auction {AuctionId} setted as Ending.", auction.Id);

        // Notification Message
        var listing = await _listingRepository.GetByIdAsync(auction.ListingId);
        if (listing is null)
        {
            _logger.LogError("Error while creating AuctionEndingNotificationMessage: Listing {ListingId} not found.", auction.ListingId);
            return;
        }
        var winningBid = auction.Bids.FirstOrDefault(b => b.Status == BidStatus.Winning);

        var auctionEndingNotificationMessage = new AuctionEndingNotificationMessage(
            ProductId: listing.ProductId,
            SellerId: listing.SellerId,
            BidCount: auction.Bids.Count,
            LastBidderId: winningBid?.BidderId);
    }
}