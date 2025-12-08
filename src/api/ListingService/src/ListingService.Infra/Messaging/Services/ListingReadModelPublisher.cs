using ListingService.App.Common.Interfaces;
using ListingService.App.Services;
using ListingService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Messages.ListingService.Common;
using Shared.Contracts.Messages.ListingService.ProductReadModel;

namespace ListingService.Infra.Messaging.Services;

internal class ListingReadModelPublisher(
    IMessageBus messageBus,
    IAuctionRepository auctionRepository,
    IListingRepository listingRepository,
    ILogger<ListingReadModelPublisher> logger) : IListingReadModelPublisher
{
    private readonly IMessageBus _messageBus = messageBus;
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly ILogger<ListingReadModelPublisher> _logger = logger;
    public async Task PublishAsync(Guid listingId, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(listingId);
        if (listing is null)
        {
            _logger.LogError("Failed to send ListingReadModelMessage: Listing not found. ListingId: {ListingId}", listingId);
            throw new Exception($"Listing not found during SendListingReadModelAsync. ListingId: {listingId}");
        }

        AuctionReadModelMessage? auctionReadModel = null;

        if (listing.AuctionId.HasValue)
        {
            var auction = await _auctionRepository.GetByIdAsync(listing.AuctionId.Value);

            if (auction is null)
            {
                _logger.LogError("Failed to send ListingReadModelMessage: Auction not found. AuctionId: {AuctionId}, ListingId: {ListingId}", listing.AuctionId.Value, listing.Id);
                throw new Exception($"Auction not found during SendListingReadModelAsync. AuctionId: {listing.AuctionId.Value}");
            }

            auctionReadModel = auction.ToAuctionReadModelMessage();
        }

        var setListingReadModelMessage = new SetListingReadModelMessage(
            ListingId: listing.Id,
            SellerId: listing.SellerId,
            ProductId: listing.ProductId,
            Status: listing.Status.ToString(),
            BuyPrice: listing.BuyPrice,
            IsAuctionActive: listing.IsAuctionActive,
            Auction: auctionReadModel,
            IsProcessingPurchase: listing.IsProcessingPurchase,
            BuyerId: listing.BuyerId,
            AuctionId: listing.AuctionId,
            ListedAt: listing.ListedAt,
            UpdatedAt: listing.UpdatedAt);

        await _messageBus.PublishAsync(setListingReadModelMessage, cancellationToken);
        _logger.LogInformation("Publishing SetListingReadModelMessage for Listing {ListingId}", listing.Id);
    }
}
