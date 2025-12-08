using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Queries.SaleQueries.GetAllDisputesByUser;

public record GetAllDisputesByUserQuery(Guid UserId, int Page, int PageSize) : IRequest<Result<IEnumerable<SaleResult>>>;