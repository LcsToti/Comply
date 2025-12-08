using ListingService.App.Common;
using ListingService.App.Common.Errors;
using ListingService.App.Common.Interfaces;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.App.Messages.AuctionStateMessages;
using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.App.Commands.AuctionCommands.EditSettings;

public class EditAuctionSettingsCommandHandler(
    IAuctionRepository auctionRepository,
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
    IDateTimeProvider dateTimeProvider,
    IMessageBus messageBus,
    ILogger<EditAuctionSettingsCommandHandler> logger) : IRequestHandler<EditSettingsCommand, Result<AuctionResult>>
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingRepository _listingRepository = listingRepository; 
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly ILogger<EditAuctionSettingsCommandHandler> _logger = logger;

    public async Task<Result<AuctionResult>> Handle(EditSettingsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling EditSettingsCommand for Auction {AuctionId}.", request.AuctionId);

        // App Logic
        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId);
        if (auction is null)
            return Result<AuctionResult>.Failure(new NotFound(nameof(Auction), request.AuctionId));

        var listing = await _listingRepository.GetByIdAsync(auction.ListingId);
        if (listing is null)
            return Result<AuctionResult>.Failure(new NotFound(nameof(Listing), auction.ListingId));

        if (listing.SellerId != request.UserId)
            return Result<AuctionResult>.Failure(new Forbidden("It is not possible to edit someone else's auction."));

        // Domain
        auction.EditSettings(
            utcNow: _dateTimeProvider.UtcNow,
            startBidValue: request.StartBidValue,
            winBidValue: request.WinBidValue,
            request.StartDate,
            request.EndDate);

        // Persist
        await _repositoryCommandsOrchestrator.UpdateAuctionAsync(auction, cancellationToken);

        // Messaging
        var startAuctionMessage = new StartAuctionMessage(auction.Id, auction.Version);
        var delay = auction.Settings.StartDate - _dateTimeProvider.UtcNow;

        _logger.LogInformation("Scheduling StartAuctionMessage for Auction {AuctionId} with delay of {Delay}", auction.Id, delay);
        await _messageBus.PublishAsync(startAuctionMessage, o => o.Delay = delay, cancellationToken);

        // Finish
        _logger.LogInformation("Auction {AuctionId} settings updated successfully", auction.Id);
        var result = auction.ToAuctionResult();
        return Result<AuctionResult>.Success(result);
    }
}