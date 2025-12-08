using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;

namespace SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsDelivered;

public record MarkSaleAsDeliveredCommand(Guid SaleId, string DeliveryCode) : IRequest<Result<SaleResult>>;