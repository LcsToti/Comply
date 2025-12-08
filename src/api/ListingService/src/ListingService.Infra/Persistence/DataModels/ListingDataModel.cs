using ListingService.Domain.ListingAggregate;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ListingService.Infra.Persistence.DataModels;

public class ListingDataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("sellerId")]
    [BsonRepresentation(BsonType.String)]
    public Guid SellerId { get; set; }

    [BsonElement("productId")]
    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public ListingStatus Status { get; set; }

    [BsonElement("buyPrice")]
    public decimal BuyPrice { get; set; }
    
    [BsonElement("IsAuctionActive")]
    public bool IsAuctionActive { get; set; }

    [BsonElement("IsProcessingPurchase")]
    public bool IsProcessingPurchase { get; set; }

    [BsonElement("buyerId")]
    [BsonRepresentation(BsonType.String)]
    public Guid? BuyerId { get; set; }

    [BsonElement("auctionId")]
    [BsonRepresentation(BsonType.String)]
    public Guid? AuctionId { get; set; }

    [BsonElement("listedAt")]
    public DateTime ListedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}