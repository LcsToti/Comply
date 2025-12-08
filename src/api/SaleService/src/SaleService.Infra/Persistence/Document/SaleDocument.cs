using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SalesService.Domain.Aggregates.SaleAggregate.Enums;

namespace SalesService.Infra.Persistence.Document;

public class SaleDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    
    [BsonElement("productId")]
    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }
    
    [BsonElement("buyerId")]   
    [BsonRepresentation(BsonType.String)]
    public Guid BuyerId { get; set; }
    
    [BsonElement("sellerId")]  
    [BsonRepresentation(BsonType.String)]
    public Guid SellerId { get; set; }
    
    [BsonElement("listingId")] 
    [BsonRepresentation(BsonType.String)]
    public Guid ListingId { get; set; }
    
    [BsonElement("paymentId")]
    [BsonRepresentation(BsonType.String)]
    public Guid PaymentId { get; set; }
    
    [BsonElement("productValue")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ProductValue { get; set; }
    
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public SaleStatus Status { get; set; }
    
    [BsonElement("deliveryStatus")]
    [BsonRepresentation(BsonType.String)]
    public DeliveryStatus DeliveryStatus { get; set; }
    
    [BsonElement("deliveryCode")]
    [BsonRepresentation(BsonType.String)]
    public string? DeliveryCode { get; set; }
    
    [BsonElement("isDeliveryCodeUsed")]
    [BsonRepresentation(BsonType.Boolean)]
    public bool IsDeliveryCodeUsed { get; set; }
    
    [BsonElement("createdAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? UpdatedAt { get; set; }
    
    [BsonElement("dispute")]
    public DisputeDocument? Dispute { get; set; }
}

public class DisputeDocument
{
    [BsonElement("id")]
    [BsonRepresentation(BsonType.String)]   
    public Guid Id { get; set; }
    
    [BsonElement("adminId")]
    [BsonRepresentation(BsonType.String)]
    public Guid? AdminId { get; set; }
    
    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public DisputeStatus Status { get; set; }
    
    [BsonElement("resolutionStatus")]
    [BsonRepresentation(BsonType.String)]
    public DisputeResolutionStatus? ResolutionStatus { get; set; }
    
    [BsonElement("reason")]
    [BsonRepresentation(BsonType.String)]
    public string Reason { get; set; }
    
    [BsonElement("resolution")]
    [BsonRepresentation(BsonType.String)]
    public string? Resolution { get; set; }
    
    [BsonElement("createdAt")]
    [BsonRepresentation(BsonType.String)]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? UpdatedAt { get; set; }
    
    [BsonElement("resolvedAt")]   
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? ResolvedAt { get; set; }
    
    [BsonElement("expiresAt")]  
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? ExpiresAt { get; set; }   
}