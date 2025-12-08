using MediatR;
using Microsoft.AspNetCore.Http;
using ProductService.Application.Responses;
using ProductService.Domain.Enums;

namespace ProductService.Application.Commands.ProductsCommands.UpdateProduct;

public record UpdateProductCommand(
    Guid ProductId,
    Guid SellerId,
    string? Title,
    string? Description,
    string? Locale,
    Dictionary<string, string>? Characteristics,
    ProductCondition? Condition,
    Categories? Category,
    DeliveryPreferences? DeliveryPreference) : IRequest<ProductResponse>;
