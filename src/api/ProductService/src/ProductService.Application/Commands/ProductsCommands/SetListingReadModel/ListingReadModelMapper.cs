using ProductService.Domain.Entities.ValueObject.ReadModels;
using Shared.Contracts.Messages.ListingService.Common;
using Shared.Contracts.Messages.ListingService.ProductReadModel;

namespace ProductService.Application.Commands.ProductsCommands.SetListingReadModel;

public static class ListingReadModelMapper
{
    public static ListingReadModel ToListingReadModel(this SetListingReadModelMessage msg)
    {
        return new ListingReadModel(
            Id: msg.ListingId,
            SellerId: msg.SellerId,
            ProductId: msg.ProductId,
            Status: msg.Status,
            BuyPrice: msg.BuyPrice,
            IsAuctionActive: msg.IsAuctionActive,
            IsProcessingPurchase: msg.IsProcessingPurchase,
            BuyerId: msg.BuyerId,
            AuctionId: msg.AuctionId,
            Auction: msg.Auction?.ToAuctionReadModel(),
            ListedAt: msg.ListedAt,
            UpdatedAt: msg.UpdatedAt);
    }

    public static AuctionReadModel ToAuctionReadModel(this AuctionReadModelMessage msg)
    {
        return new AuctionReadModel(
            Id: msg.AuctionId,
            ListingId: msg.ListingId,
            Bids: msg.Bids?.Select(b => b.ToBidReadModel()).ToList() ?? [],
            Status: msg.Status,
            Settings: msg.Settings.ToAuctionSettingsReadModel(),
            IsProcessingBid: msg.IsProcessingBid,
            EditedAt: msg.EditedAt,
            StartedAt: msg.StartedAt,
            EndedAt: msg.EndedAt);
    }

    public static AuctionSettingsReadModel ToAuctionSettingsReadModel(this AuctionSettingsReadModelMessage msg)
    {
        return new AuctionSettingsReadModel(
            StartBidValue: msg.StartBidValue,
            WinBidValue: msg.WinBidValue,
            StartDate: msg.StartDate,
            EndDate: msg.EndDate);
    }

    public static BidReadModel ToBidReadModel(this BidReadModelMessage msg)
    {
        return new BidReadModel(
            Id: msg.BidId,
            AuctionId: msg.AuctionId,
            BidderId: msg.BidderId,
            Value: msg.Value,
            Status: msg.Status,
            BiddedAt: msg.BiddedAt,
            OutbiddedAt: msg.OutbiddedAt,
            WonAt: msg.WonAt);
    }
}