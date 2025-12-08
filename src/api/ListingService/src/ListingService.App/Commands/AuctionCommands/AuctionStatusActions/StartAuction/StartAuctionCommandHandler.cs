using DnsClient.Internal;
using ListingService.App.Common;
using ListingService.App.Common.Interfaces;
using ListingService.App.Messages.AuctionStateMessages;
using ListingService.Domain.Common;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.ListingService.Notifications.Auction;

namespace ListingService.App.Commands.AuctionCommands.AuctionStatusActions.StartAuction;

public class StartAuctionCommandHandler(
    IAuctionRepository auctionRepository,
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
    IMessageBus messageBus,
    IDateTimeProvider dateTimeProvider,
    ILogger<StartAuctionCommandHandler> logger) : IRequestHandler<StartAuctionCommand>
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<StartAuctionCommandHandler> _logger = logger;

    public async Task Handle(StartAuctionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling StartAuctionCommand for Auction {AuctionId}.", request.AuctionId);

        var utcNow = _dateTimeProvider.UtcNow;

        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId);
        if (auction is null)
        {
            _logger.LogError("Error while consuming StartAuctionCommand: Auction {Id} not found.", request.AuctionId);
            return;
        }

        var listing = await _listingRepository.GetByIdAsync(auction.ListingId);
        if (listing is null)
        {
            _logger.LogError("Error while consuming StartAuctionCommand: Listing {Id} not found.", auction.ListingId);
            return;
        }

        if (auction.Version != request.Version)
        {
            _logger.LogError(
                "Fail while consuming StartAuctionCommand: Auction version is {AuctionVersion} and Message Version Is {MessageVersion}, aborting auction.Start().",
                auction.Version,
                request.Version);
            return;
        }

        // Domain
        auction.Start(utcNow);

        // Domain Effects
        listing.SetAuctionActive(utcNow);

        // Persist
        await _repositoryCommandsOrchestrator.UpdateAuctionAsync(auction, cancellationToken);
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);

        // RabbitMQ
        var setAuctionAsEndingMessage = new SetAuctionAsEndingMessage(auction.Id, auction.Version);

        var delay = auction.Settings.EndDate - _dateTimeProvider.UtcNow - DomainConstants.RemainingTimeToSetEnding;

        _logger.LogInformation("Scheduling SetAuctionAsEndingMessage for Auction {AuctionId} with delay of {Delay}.", auction.Id, delay);
        await _messageBus.PublishAsync(setAuctionAsEndingMessage, ctx => { ctx.Delay = delay; }, cancellationToken);

        _logger.LogInformation("StartAuctionCommand success: Auction {AuctionId} started.", auction.Id);

        // Notification Message
        var auctionStartedNotificationMessage = new AuctionStartedNotificationMessage(
            ProductId: listing.ProductId,
            SellerId: listing.SellerId,
            StartedAt: auction.StartedAt!.Value,
            MinBidValue: auction.Settings.StartBidValue);
        await _messageBus.PublishAsync(auctionStartedNotificationMessage, cancellationToken);
        _logger.LogInformation("AuctionStartedNotificationMessage published for Auction {AuctionId}.", auction.Id);
    }
}
