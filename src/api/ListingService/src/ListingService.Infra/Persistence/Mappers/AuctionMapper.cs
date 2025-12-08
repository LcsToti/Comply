using ListingService.Domain.AuctionAggregate.Entities;
using ListingService.Domain.AuctionAggregate.ValueObjects;
using ListingService.Infra.Persistence.DataModels;
using Shared.Contracts.Messages.ListingService.Common;
using Shared.Contracts.Messages.ListingService.ProductReadModel;

namespace ListingService.Infra.Persistence.Mappers;

public static class AuctionMapper
{
    public static AuctionDataModel ToDataModel(this Auction entity) => new()
    {
        Id = entity.Id,
        ListingId = entity.ListingId,
        Bids = [.. entity.Bids.Select(b => new BidDataModel
        {
            Id = b.Id,
            AuctionId = b.AuctionId,
            BidderId = b.BidderId,
            PaymentId = b.PaymentId,
            Value = b.Value,
            Status = b.Status,
            BiddedAt = b.BiddedAt,
            OutbiddedAt = b.OutbiddedAt,
            WonAt = b.WonAt
        })],
        Status = entity.Status,
        Settings = new AuctionSettingsDataModel
        {
            StartBidValue = entity.Settings.StartBidValue,
            WinBidValue = entity.Settings.WinBidValue,
            StartDate = entity.Settings.StartDate,
            EndDate = entity.Settings.EndDate
        },
        Version = entity.Version,
        IsProcessingBid = entity.IsProcessingBid,
        EditedAt = entity.EditedAt,
        StartedAt = entity.StartedAt,
        EndedAt = entity.EndedAt
    };

    public static Auction ToDomain(this AuctionDataModel dm)
    {
        var settings = AuctionSettings.Restore(dm.Settings.StartBidValue, dm.Settings.WinBidValue, dm.Settings.StartDate, dm.Settings.EndDate);

        var bids = dm.Bids.Select(b =>
            Bid.Restore(
                b.Id,
                b.AuctionId,
                b.BidderId,
                b.PaymentId,
                b.Value,
                b.Status,
                b.BiddedAt,
                b.OutbiddedAt,
                b.WonAt)).ToList();

        return Auction.Restore(
            dm.Id,
            dm.ListingId,
            settings,
            dm.Status,
            dm.Version,
            dm.IsProcessingBid,
            dm.EditedAt,
            dm.StartedAt,
            dm.EndedAt,
            bids);
    }
}