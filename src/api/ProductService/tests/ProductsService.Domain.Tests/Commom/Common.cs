using ProductService.Domain.Entities;
using ProductService.Domain.Enums;
using ProductService.Domain.Factories;

namespace ProductsService.Domain.Tests.Commom;

public class Common
{
    public static Product CreateTestProduct(Guid sellerId)
    {
        return ProductFactory.Create(
            sellerId,
            "Título Original",
            "Descrição Original",
            "pt-BR",
            new Dictionary<string, string> { { "Cor", "Azul" } },
            ProductCondition.New,
            Categories.Electronics,
            DeliveryPreferences.Both
        );
    }
}