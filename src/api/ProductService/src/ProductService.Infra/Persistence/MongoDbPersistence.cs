using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ProductService.Domain.Entities;
using ProductService.Domain.Entities.ValueObjects;

namespace ProductService.Infrastructure.Persistence;

public static class MongoDbPersistence
{
    public static void Configure()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        BsonClassMap.RegisterClassMap<Product>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);

            cm.MapIdProperty(p => p.ProductId)
                .SetSerializer(new GuidSerializer(BsonType.String));

            cm.MapMember(p => p.SellerId)
                .SetSerializer(new GuidSerializer(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<Question>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            cm.MapMember(q => q.QuestionId)
                .SetSerializer(new GuidSerializer(BsonType.String));
            cm.MapMember(q => q.UserId)
                .SetSerializer(new GuidSerializer(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<Answer>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });
    }
}