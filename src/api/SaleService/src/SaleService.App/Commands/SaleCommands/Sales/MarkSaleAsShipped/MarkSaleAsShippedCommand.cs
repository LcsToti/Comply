using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsShipped;

public record MarkSaleAsShippedCommand(Guid SaleId, Guid UserId) : IRequest<Result<SaleResult>>;