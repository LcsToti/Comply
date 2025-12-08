using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Queries.SaleQueries.GetAllSalesByUser;

public record GetMySalesQuery(Guid UserId, int Page, int PageSize) : IRequest<Result<IEnumerable<SaleResult>>>;