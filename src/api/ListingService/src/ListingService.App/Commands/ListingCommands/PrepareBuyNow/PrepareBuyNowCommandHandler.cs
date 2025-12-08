using ListingService.App.Common;
using ListingService.App.Common.Errors;
using ListingService.App.Common.Interfaces;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.App.Messages.PaymentTimeoutMessages;
using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.ListingService.Payments.Purchase;

namespace ListingService.App.Commands.ListingCommands.PrepareBuyNow;

public class PrepareBuyNowCommandHandler(
        IListingRepository listingRepository,
        RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
        IDateTimeProvider dateTimeProvider,
        IMessageBus messageBus,
        ILogger<PrepareBuyNowCommandHandler> logger) : IRequestHandler<PrepareBuyNowCommand, Result<ListingResult>>
{
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IMessageBus _messageBus = messageBus;
    private readonly ILogger<PrepareBuyNowCommandHandler> _logger = logger;

    public async Task<Result<ListingResult>> Handle(PrepareBuyNowCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling PrepareBuyNowCommand for Listing {Id} by User {UserId}", request.ListingId, request.UserId);

        // Atomic Lock
        var lockedSuccess = await _listingRepository.AtomicPrepareBuyNowAsync(request.ListingId);
        if (!lockedSuccess)
        {
            var tryGetAuction = await _listingRepository.GetByIdAsync(request.ListingId);
            if (tryGetAuction is null)
            {
                await _listingRepository.ReleaseBuyNowLockAsync(request.ListingId);
                return Result<ListingResult>.Failure(new NotFound(nameof(Listing), request.ListingId));
            }

            await _listingRepository.ReleaseBuyNowLockAsync(request.ListingId);
            return Result<ListingResult>.Failure(new InvalidBidOperation("Can't buy while another purchase request is being processed."));
        }

        // App Logic
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);
        if (listing is null)
        {
            await _listingRepository.ReleaseBuyNowLockAsync(request.ListingId);
            return Result<ListingResult>.Failure(new NotFound(nameof(Listing), request.ListingId));
        }

        if (listing.SellerId == request.UserId)
        {
            await _listingRepository.ReleaseBuyNowLockAsync(request.ListingId);
            return Result<ListingResult>.Failure(new InvalidPurchaseOperation("It's not possible to buy your own product."));
        }

        // Domain
        try
        {
            listing.PrepareBuyNow();
        }
        catch
        {
            await _listingRepository.ReleaseBuyNowLockAsync(listing.Id);
            throw;
        }
        // Persist
        await _repositoryCommandsOrchestrator.UpdateListingAsync(listing, cancellationToken);

        // Messaging
        var expiresAt = _dateTimeProvider.UtcNow + AppConstants.TotalMessageTimeWindow;

        var buyPendingEvent = new PurchasePaymentRequestMessage(
            ListingId: listing.Id,
            BuyerId: request.UserId,
            ExpiresAt: expiresAt,
            Value: listing.BuyPrice,
            PaymentMethod: request.PaymentMethod,
            SellerId: listing.SellerId);

        var buyPendingTimeoutEvent = new BuyPendingTimeoutMessage(ListingId: listing.Id);

        _logger.LogInformation("Publishing BuyPendingMessage for Listing {Id} with TTL of {TimeToLive} ", listing.Id, AppConstants.TimeToLive);
        await _messageBus.PublishAsync(buyPendingEvent, o => o.TimeToLive = AppConstants.TimeToLive, cancellationToken);

        _logger.LogInformation("Scheduling buyPendingTimeoutEvent for Listing {Id} with delay of {Delay}", listing.Id, AppConstants.TotalMessageTimeWindow);
        await _messageBus.PublishAsync(buyPendingTimeoutEvent, o => o.Delay = AppConstants.TotalMessageTimeWindow, cancellationToken);

        // Finish
        _logger.LogInformation("PrepareBuyNow {Id} created successfully", listing.Id);
        return Result<ListingResult>.Success(listing.ToListingResult());
    }
}