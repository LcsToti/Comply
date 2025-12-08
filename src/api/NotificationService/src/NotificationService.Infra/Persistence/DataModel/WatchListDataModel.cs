using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Infra.Persistence.DataModel;

public class WatchListDataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public List<Guid> ProductsWatching { get; set; }
}