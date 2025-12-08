using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Aggregates.SaleAggregate.Enums;

namespace SalesService.API.Responses;

public record SaleResponse(
    Guid Id,
    Guid ProductId,
    Guid ListingId,
    Guid PaymentId,
    Guid BuyerId,
    Guid SellerId,
    decimal ProductValue,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    SaleStatus Status,
    DeliveryStatus DeliveryStatus, 
    Dispute? Dispute);