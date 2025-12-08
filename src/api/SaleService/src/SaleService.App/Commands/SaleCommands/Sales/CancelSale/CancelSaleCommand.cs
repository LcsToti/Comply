using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Commands.SaleCommands.Sales.CancelSale;

public record CancelSaleCommand(Guid SaleId, string Role) : IRequest<Result<SaleResult>>;