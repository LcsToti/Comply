using MediatR;
using SalesService.App.Common;

namespace SalesService.App.Queries.SaleQueries.GetSaleDeliveryCode;

public record GetSaleDeliveryCodeQuery(Guid SaleId, Guid UserId) : IRequest<Result<string>>;