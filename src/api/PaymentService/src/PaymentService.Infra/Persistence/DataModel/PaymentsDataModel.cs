using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Payments.Domain.Aggregates.PaymentAggregate.Enums;

namespace Payments.Infra.Persistence.DataModel
{
    public class PaymentDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("withdrawalStatus")]
        [BsonRepresentation(BsonType.String)]
        public WithdrawalStatus WithdrawalStatus { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public PaymentStatus Status { get; set; }
        
        [BsonElement("paymentMethod")]
        public string? PaymentMethod { get; set; }

        [BsonElement("amount")]
        public AmountDataModel Amount { get; set; }

        [BsonElement("gateway")]
        public GatewayDataModel Gateway { get; set; }
        
        [BsonElement("payerId")]
        [BsonRepresentation(BsonType.String)]
        public Guid PayerId { get; set; }
        
        [BsonElement("sellerId")]
        [BsonRepresentation(BsonType.String)]
        public Guid? SellerId { get; set; }

        [BsonElement("sourceId")]
        [BsonRepresentation(BsonType.String)]
        public Guid SourceId { get; set; }

        [BsonElement("timestamps")]
        public TimestampsDataModel Timestamps { get; set; }

        [BsonElement("refunds")]
        public List<RefundDataModel> Refunds { get; set; } = [];
    }
    
    public class AmountDataModel
    {
        [BsonElement("total")]
        public decimal Total { get; set; }
        
        [BsonElement("fee")]
        public decimal Fee { get; set; }
        
        [BsonElement("net")]
        public decimal Net { get; set; }
        
        [BsonElement("currency")]
        public string Currency { get; set; }
    }
    public class GatewayDataModel
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("apiPaymentId")]
        public string ApiPaymentId { get; set; }
        
        [BsonElement("apiChargeId")]
        public string? ApiChargeId { get; set; }
    }
    public class TimestampsDataModel
    {
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [BsonElement("processedAt")]
        public DateTime? ProcessedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        
        [BsonElement("withdrawnAt")]
        public DateTime? WithdrawnAt { get; set; }
    }
    public class RefundDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("apiRefundId")]
        public string ApiRefundId { get; set; }

        [BsonElement("refundAmount")]
        public decimal RefundAmount { get; set; }

        [BsonElement("reason")]
        public string Reason { get; set; }
        
        [BsonElement("refundStatus")]
        public string RefundStatus { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}