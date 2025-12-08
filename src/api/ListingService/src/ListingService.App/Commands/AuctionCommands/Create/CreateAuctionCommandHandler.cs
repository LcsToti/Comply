using ListingService.App.Common;
using ListingService.App.Common.Errors;
using ListingService.App.Common.Interfaces;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.App.Messages.AuctionStateMessages;
using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.AuctionAggregate.ValueObjects;
using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.App.Commands.AuctionCommands.Create;

public class CreateAuctionCommandHandler(
        IListingRepository listingRepository,
        RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
        IDateTimeProvider dateTimeProvider,
        IMessageBus messageBus,
        ILogger<CreateAuctionCommandHandler> logger) : IRequestHandler<CreateAuctionCommand, Result<AuctionResult>>
{
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly ILogger<CreateAuctionCommandHandler> _logger = logger;

    public async Task<Result<AuctionResult>> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateAuctionCommand for Listing {ListingId}.", request.ListingId);

        // App Logic
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);
        if (listing is null)
            return Result<AuctionResult>.Failure(new NotFound(nameof(Listing), request.ListingId));

        if (listing.AuctionId is not null)
            return Result<AuctionResult>.Failure(new Conflict("Listing is already attached to an auction."));

        if (listing.Status != ListingStatus.Available)
            return Result<AuctionResult>.Failure(new InvalidListingStatus(listing.Status.ToString()));

        if (listing.SellerId != request.UserId)
            return Result<AuctionResult>.Failure(new Forbidden("It is not possible to create an auction for someone else's listing."));

        // Domain
        var auctionSettings = AuctionSettings.Create(request.StartBidValue, request.WinBidValue, request.StartDate, request.EndDate, _dateTimeProvider.UtcNow);
        var auction = Auction.Create(request.ListingId, auctionSettings);

        // Domain Effects:
        listing.AttachAuction(auction.Id, _dateTimeProvider.UtcNow);

        // Persist
        await _repositoryCommandsOrchestrator.AddAuctionAsync(auction, cancellationToken);
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);

        // Messaging
        var startAuctionMessage = new StartAuctionMessage(auction.Id, auction.Version);
        var delay = auctionSettings.StartDate - _dateTimeProvider.UtcNow;

        _logger.LogInformation("Scheduling StartAuctionMessage for Auction {AuctionId} with delay of {Delay}", auction.Id, delay);
        await _messageBus.PublishAsync(startAuctionMessage, o => o.Delay = delay, cancellationToken);

        // Finish
        _logger.LogInformation("Auction {AuctionId} created successfully", auction.Id);
        var result = auction.ToAuctionResult();
        return Result<AuctionResult>.Success(result);
    }
}