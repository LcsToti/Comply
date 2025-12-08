using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Domain.Enums;

namespace NotificationService.Infra.Persistence.DataModel;

public class NotificationDataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]   
    public Guid UserId { get; set; }
    
    [BsonRepresentation(BsonType.String)]  
    public NotificationType Type { get; set; }
    
    public string Message { get; set; }
    
    public bool Read { get; set; }
    
    public DateTime CreatedAt { get; set; }
}