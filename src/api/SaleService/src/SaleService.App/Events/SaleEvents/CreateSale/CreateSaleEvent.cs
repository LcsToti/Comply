using MediatR;

namespace SalesService.App.Events.SaleEvents.CreateSale;

public record CreateSaleEvent(Guid ProductId, Guid BuyerId, Guid SellerId, Guid ListingId, Guid PaymentId, decimal ProductValue) : IRequest;