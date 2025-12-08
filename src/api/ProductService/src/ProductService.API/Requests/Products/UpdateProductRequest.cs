using ProductService.Domain.Enums;

namespace ProductService.API.Requests.Products;

public record UpdateProductRequest(
    string? Title,
    string? Description,
    string? Locale,
    Dictionary<string, string>? Characteristics,
    ProductCondition? Condition,
    Categories? Category,
    DeliveryPreferences? DeliveryPreference,
    List<IFormFile>? ImageUrls);
