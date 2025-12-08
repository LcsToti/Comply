using ListingService.Domain.AuctionAggregate.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ListingService.Infra.Persistence.DataModels;

public class AuctionDataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("listingId")]
    [BsonRepresentation(BsonType.String)]
    public Guid ListingId { get; set; }

    [BsonElement("bids")]
    public List<BidDataModel> Bids { get; set; } = [];

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public AuctionStatus Status { get; set; }

    [BsonElement("auctionSettings")]
    public required AuctionSettingsDataModel Settings { get; set; }

    [BsonElement("version")]
    public int Version { get; set; }

    [BsonElement("isProcessingBid")]
    public bool IsProcessingBid { get; set; }

    [BsonElement("editedAt")]
    public DateTime? EditedAt { get; set; }

    [BsonElement("startedAt")]
    public DateTime? StartedAt { get; set; }

    [BsonElement("endedAt")]
    public DateTime? EndedAt { get; set; }
}

public class BidDataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("auctionId")]
    [BsonRepresentation(BsonType.String)]
    public Guid AuctionId { get; set; }

    [BsonElement("bidderId")]
    [BsonRepresentation(BsonType.String)]
    public Guid BidderId { get; set; }

    [BsonElement("paymentId")]
    [BsonRepresentation(BsonType.String)]
    public Guid PaymentId { get; set; }

    [BsonElement("value")]
    public decimal Value { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public BidStatus Status { get; set; }

    [BsonElement("biddedAt")]
    public DateTime BiddedAt { get; set; }

    [BsonElement("outbiddedAt")]
    public DateTime? OutbiddedAt { get; set; }

    [BsonElement("wonAt")]
    public DateTime? WonAt { get; set; }
}

public class AuctionSettingsDataModel
{
    [BsonElement("startBidValue")]
    public decimal StartBidValue { get; set; }

    [BsonElement("winBidValue")]
    public decimal WinBidValue { get; set; }

    [BsonElement("startDate")]
    public DateTime StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime EndDate { get; set; }
}