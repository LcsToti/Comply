using ListingService.App.Common;
using ListingService.App.Common.Interfaces;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.App.Commands.AuctionCommands.AbortNewBid;

public class AbortNewBidCommandHandler(
    IAuctionRepository auctionRepository,
    RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
    IDateTimeProvider dateTimeProvider,
    ILogger<AbortNewBidCommandHandler> logger) : IRequestHandler<AbortNewBidCommand>
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<AbortNewBidCommandHandler> _logger = logger;

    public async Task Handle(AbortNewBidCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling AbortNewBidCommand for Auction {Id}", request.AuctionId);

        var utcNow = _dateTimeProvider.UtcNow;

        if (request.ExpiresAt is not null && utcNow > request.ExpiresAt)
        {
            _logger.LogError("Error while aborting purchase on auction {Id}: Expired", request.AuctionId);
            return;
        }

        var auction = await _auctionRepository.GetByIdAsync(request.AuctionId);
        if (auction is null)
        {
            _logger.LogError("Error while aborting new bid on auction {Id}: Auction not found", request.AuctionId);
            return;
        }

        // Domain
        try
        {
            auction.AbortNewBid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while aborting new bid on auction {Id}", request.AuctionId);
            return;
        }

        // Persist
        await _repositoryCommandsOrchestrator.UpdateAuctionAsync(auction, cancellationToken);
        
        _logger.LogInformation("Successfully aborted new bid on auction {Id}", request.AuctionId);
    }
}