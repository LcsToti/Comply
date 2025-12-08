using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Queries.SaleQueries.GetAllSales;

public record GetAllSalesQuery() : IRequest<Result<List<SaleResult>>>;