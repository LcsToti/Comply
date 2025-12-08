using MongoDB.Driver;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infra.Persistence.Documents;

namespace UserService.Infra.Persistence.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IMongoCollection<RefreshTokenDocument> _collection;

    public AuthRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<RefreshTokenDocument>("RefreshTokens");
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var tokenIndex = Builders<RefreshTokenDocument>.IndexKeys.Ascending(t => t.Token);
        _collection.Indexes.CreateOne(new CreateIndexModel<RefreshTokenDocument>(tokenIndex, new CreateIndexOptions { Unique = true }));

        var userIndex = Builders<RefreshTokenDocument>.IndexKeys.Ascending(t => t.UserId);
        _collection.Indexes.CreateOne(new CreateIndexModel<RefreshTokenDocument>(userIndex));
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct)
    {
        var filter = Builders<RefreshTokenDocument>.Filter.Eq(d => d.Token, token);
        var doc = await _collection.Find(filter).FirstOrDefaultAsync(ct);
        return doc?.ToDomain();
    }

    public async Task SaveRefreshTokenAsync(RefreshToken token, CancellationToken ct)
    {
        var doc = RefreshTokenDocument.FromDomain(token);
        await _collection.InsertOneAsync(doc, cancellationToken: ct);
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken ct)
    {
        var filter = Builders<RefreshTokenDocument>.Filter.Eq(d => d.Token, token);
        var update = Builders<RefreshTokenDocument>.Update.Set(d => d.IsRevoked, true);
        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}
