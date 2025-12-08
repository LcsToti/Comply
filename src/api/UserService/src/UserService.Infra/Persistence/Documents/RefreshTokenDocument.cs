using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserService.Domain.Entities;

namespace UserService.Infra.Persistence.Documents;

public class RefreshTokenDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonRepresentation(BsonType.String)]
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }

    public static RefreshTokenDocument FromDomain(RefreshToken domain) => new()
    {
        Id = domain.Id.ToString(),
        UserId = domain.UserId.ToString(),
        Token = domain.Token,
        CreatedAt = domain.CreatedAt,
        ExpiresAt = domain.ExpiresAt,
        IsRevoked = domain.IsRevoked
    };

    public RefreshToken ToDomain() => new(
        id: Guid.Parse(Id),
        userId: Guid.Parse(UserId),
        token: Token,
        createdAt: CreatedAt,
        expiresAt: ExpiresAt,
        isRevoked: IsRevoked);
}
