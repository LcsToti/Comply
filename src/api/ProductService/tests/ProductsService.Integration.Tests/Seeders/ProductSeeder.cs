using MongoDB.Driver;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;
using ProductService.Domain.Factories;

namespace ProductsService.Integration.Tests.Seeders;

public static class ProductSeeder
{
    public static async Task<Product> SeedInappropriateProduct(IMongoCollection<Product> collection, Guid? userId = null)
    {
        var sellerId = userId ?? Guid.NewGuid();

        Product product = ProductFactory.Create(
            sellerId: sellerId,
            title: "Sample Product",
            description: "This is a sample product description.",
            locale: "Sao Paulo",
            characteristics: new Dictionary<string, string>
            {
                { "Color", "Red" },
                { "Size", "Medium" }
            },
            condition: ProductCondition.New,
            category: Categories.Electronics,
            deliveryPreference: DeliveryPreferences.PickupPoint
        );

        var imageUrls = new List<string>
        {
            "https://example.com/image1.jpg",
            "https://example.com/image2.jpg"
        };

        product.AddImages(sellerId, imageUrls);

        await collection.InsertOneAsync(product);
        return product;
    }
}
