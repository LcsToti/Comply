using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infra.Persistence.Documents;

namespace UserService.Infra.Persistence.Repositories;

public class MongoDbUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _usersCollection;

    public MongoDbUserRepository(IMongoDatabase database)
    {
        _usersCollection = database.GetCollection<UserDocument>("Users");
        CreateIndexes();
    }
    private void CreateIndexes()
    {
        var indexKeys = Builders<UserDocument>.IndexKeys.Ascending(doc => doc.Email);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<UserDocument>(indexKeys, indexOptions);

        _usersCollection.Indexes.CreateOne(indexModel);

        var idIndexKeys = Builders<UserDocument>.IndexKeys.Ascending(doc => doc.Id);
        var idIndexModel = new CreateIndexModel<UserDocument>(idIndexKeys);

        _usersCollection.Indexes.CreateOne(idIndexModel);
    }

    public async Task AddAsync(User user)
    {
        var userDocument = UserDocument.FromDomain(user);
        await _usersCollection.InsertOneAsync(userDocument);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<UserDocument>.Filter.Eq(doc => doc.Email, email.ToLower());

        var userDocument = await _usersCollection
            .Find(filter)
            .FirstOrDefaultAsync();

        return userDocument?.ToDomain();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var filter = Builders<UserDocument>.Filter.Eq(doc => doc.Id, id);

        var userDocument = await _usersCollection
            .Find(filter)
            .FirstOrDefaultAsync();

        return userDocument?.ToDomain();
    }

    public async Task UpdateAsync(User user)
    {
        var userDocument = UserDocument.FromDomain(user);

        var filter = Builders<UserDocument>.Filter.Eq(doc => doc.Id, user.Id);

        var result = await _usersCollection.ReplaceOneAsync(filter, userDocument);

        if (result.MatchedCount == 0)
        {
            throw new Exception("Usuário não encontrado para atualização.");
        }
    }

    public async Task<bool> UpdateDeliveryAddressAsync(string userId, string addressId, DeliveryAddress address)
    {
        var filter = Builders<UserDocument>.Filter.And(
            Builders<UserDocument>.Filter.Eq(u => u.Id, userId),
            Builders<UserDocument>.Filter.ElemMatch(u => u.DeliveryAddresses, d => d.Id == addressId)
        );

        var update = Builders<UserDocument>.Update.Set("DeliveryAddresses.$", address);

        var result = await _usersCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveDeliveryAddressAsync(string userId, string addressId)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserDocument>.Update.PullFilter(
            u => u.DeliveryAddresses, 
            d => d.Id == addressId
        );

        var result = await _usersCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<DeliveryAddress?> GetDeliveryAddressByIdAsync(string userId, string addressId)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);
        
        var projection = Builders<UserDocument>.Projection
            .ElemMatch(u => u.DeliveryAddresses, d => d.Id == addressId)
            .Exclude("_id");

        var userDocument = await _usersCollection.Find(filter).Project(projection).FirstOrDefaultAsync();

        if (userDocument != null && userDocument.Contains("DeliveryAddresses"))
        {
            var addresses = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<DeliveryAddress>>(userDocument["DeliveryAddresses"].AsBsonArray.ToJson());
            return addresses.FirstOrDefault();
        }

        return null;
    }
    public async Task<IEnumerable<DeliveryAddress>> GetDeliveryAddressesAsync(string userId)
    {
        var filter = Builders<UserDocument>.Filter.Eq(doc => doc.Id, userId);
        
        var projection = Builders<UserDocument>.Projection
            .Include(doc => doc.DeliveryAddresses)
            .Exclude("_id");

        var userDocument = await _usersCollection
            .Find(filter)
            .Project<UserDocument>(projection)
            .FirstOrDefaultAsync();

        if (userDocument == null)
        {
            throw new Exception("Não existem endereços registrados."); 
        }

        return userDocument.DeliveryAddresses;
    }

    public async Task<long> GetAllUsersCountAsync()
    {
        long count = await _usersCollection.CountDocumentsAsync(new BsonDocument());
        return count;
    }
}