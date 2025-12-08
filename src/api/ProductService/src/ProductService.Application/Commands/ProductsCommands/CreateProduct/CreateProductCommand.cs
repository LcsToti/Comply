using MediatR;
using Microsoft.AspNetCore.Http;
using ProductService.Application.Responses;
using ProductService.Domain.Enums;

namespace ProductService.Application.Commands.ProductsCommands.CreateProduct;

public record CreateProductCommand(
    Guid SellerId,
    string Title,
    string Description, 
    List<IFormFile> ImageUrls,
    string Locale,
    Dictionary<string, string> Characteristics,
    ProductCondition Condition,
    Categories Category,
    DeliveryPreferences DeliveryPreference,
    bool IsTest,
    string UserRole) : IRequest<ProductResponse>;
