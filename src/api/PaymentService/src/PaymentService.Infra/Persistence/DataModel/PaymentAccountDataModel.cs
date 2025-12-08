using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Payments.Domain.Aggregates.PaymentAccountAggregate;

namespace Payments.Infra.Persistence.DataModel;

public class PaymentAccountDataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    
    [BsonElement("customerId")]
    public string CustomerId { get; set; }
    
    [BsonElement("connectedAccountId")]
    public string ConnectedAccountId { get; set; }
    
    [BsonElement("accountStatus")]
    [BsonRepresentation(BsonType.String)]
    public PaymentAccountStatus AccountStatus { get; set; }
}