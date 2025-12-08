using ListingService.App.Common;
using ListingService.App.Common.Errors;
using ListingService.App.Common.Interfaces;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.App.Messages.PaymentTimeoutMessages;
using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.ListingService.Payments.Bid;

namespace ListingService.App.Commands.AuctionCommands.PrepareNewBid;

public class PrepareNewBidCommandHandler(
    IAuctionRepository auctionRepository,
    IListingRepository listingRepository,
    RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
    IDateTimeProvider dateTimeProvider,
    IMessageBus messageBus,
    ILogger<PrepareNewBidCommandHandler> logger) : IRequestHandler<PrepareNewBidCommand, Result<AuctionResult>>
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly ILogger<PrepareNewBidCommandHandler> _logger = logger;

    public async Task<Result<AuctionResult>> Handle(PrepareNewBidCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PrepareNewBidCommand for Auction {AuctionId}.", request.AuctionId);

        // Atomic Lock
        var lockedSuccess = await _auctionRepository.AtomicPrepareNewBidAsync(request.AuctionId);
        if (!lockedSuccess)
        {
            var tryGetAuction = await _auctionRepository.GetByIdAsync(request.AuctionId);
            if (tryGetAuction is null)
            {
                await _auctionRepository.ReleaseNewBidLockAsync(request.AuctionId);
                return Result<AuctionResult>.Failure(new NotFound(nameof(Auction), request.AuctionId));
            }

            await _auctionRepository.ReleaseNewBidLockAsync(request.AuctionId);
            return Result<AuctionResult>.Failure(new InvalidBidOperation("Can't place bids while a bid request is being processed."));
        }

        // App Logic
        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId);
        if (auction is null)
        {
            await _auctionRepository.ReleaseNewBidLockAsync(request.AuctionId);
            return Result<AuctionResult>.Failure(new NotFound(nameof(Auction), request.AuctionId));
        }

        var listing = await _listingRepository.GetByIdAsync(auction.ListingId);
        if (listing is null)
        {
            await _auctionRepository.ReleaseNewBidLockAsync(request.AuctionId);
            return Result<AuctionResult>.Failure(new NotFound(nameof(Listing), auction.ListingId));
        }

        if (listing.SellerId == request.UserId)
        {
            await _auctionRepository.ReleaseNewBidLockAsync(request.AuctionId);
            return Result<AuctionResult>.Failure(new InvalidBidOperation("It is not possible to place bids on your own auction."));
        }

        // Domain
        try
        {
            auction.PrepareNewBid(request.BidValue);
        }
        catch
        {
            await _auctionRepository.ReleaseNewBidLockAsync(auction.Id);
            throw;
        }

        // Persist
        await _repositoryCommandsOrchestrator.UpdateAuctionAsync(auction, cancellationToken);
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);

        // Messaging
        var expiresAt = _dateTimeProvider.UtcNow + AppConstants.TotalMessageTimeWindow;

        var newBidPendingMessage = new BidPaymentRequestMessage(
            AuctionId: auction.Id,
            BidderId: request.UserId,
            ExpiresAt: expiresAt,
            Value: request.BidValue,
            PaymentMethod: request.PaymentMethod,
            SellerId: listing.SellerId);

        var newBidPendingTimeoutMessage = new NewBidPendingTimeoutMessage(AuctionId: auction.Id);

        _logger.LogInformation("Publishing NewBidPendingMessage for Auction {AuctionId} with TTL of {TTL}", auction.Id, AppConstants.TimeToLive);
        await _messageBus.PublishAsync(newBidPendingMessage, o => o.TimeToLive = AppConstants.TimeToLive, cancellationToken);

        _logger.LogInformation("Scheduling NewBidPendingTimeoutMessage for Auction {AuctionId} with delay of {Delay}", auction.Id, AppConstants.TotalMessageTimeWindow);
        await _messageBus.PublishAsync(newBidPendingTimeoutMessage, o => o.Delay = AppConstants.TotalMessageTimeWindow, cancellationToken);

        // Finish
        _logger.LogInformation("Published newBidPendingMessage for auction {AuctionId}", auction.Id);
        return Result<AuctionResult>.Success(auction.ToAuctionResult());
    }
}
