using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Domain.Enums;

namespace NotificationService.Infra.Persistence.DataModel;

public class SupportTicketDataModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]   
    public Guid Id {  get; set; }
    
    [BsonRepresentation(BsonType.String)] 
    public Guid UserId { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public TicketStatus Status { get; set; }
    
    public DateTime LastUpdateAt { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid? AssignedToAdminId { get; set; }

    public List<CommentDataModel> _comments = new List<CommentDataModel>();
    public IReadOnlyList<CommentDataModel> Comments => _comments.AsReadOnly();
}

public class CommentDataModel
{
    [BsonRepresentation(BsonType.String)]
    public Guid AuthorId { get; set; }
    
    public string Content { get; set; }
    
    public DateTime CreatedAt { get; set; }
}