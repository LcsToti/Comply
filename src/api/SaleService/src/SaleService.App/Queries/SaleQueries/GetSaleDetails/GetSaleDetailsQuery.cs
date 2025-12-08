using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Queries.SaleQueries.GetSaleDetails;

public record GetSaleDetailsQuery(Guid SaleId, Guid UserId, string Role) : IRequest<Result<SaleResult>>;