namespace UserService.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }

    public RefreshToken(Guid userId, string token, DateTime createdAt, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    internal RefreshToken(Guid id, Guid userId, string token, DateTime createdAt, DateTime expiresAt, bool isRevoked)
    {
        Id = id;
        UserId = userId;
        Token = token;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        IsRevoked = isRevoked;
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive() => !IsRevoked && !IsExpired();
}
