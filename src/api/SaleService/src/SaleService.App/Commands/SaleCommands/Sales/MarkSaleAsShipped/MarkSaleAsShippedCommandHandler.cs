using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common;
using SalesService.App.Common.Errors;
using SalesService.App.Common.Results;
using SalesService.App.Common.Results.Mappers;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;

namespace SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsShipped;

public class MarkSaleAsShippedCommandHandler : IRequestHandler<MarkSaleAsShippedCommand, Result<SaleResult>>
{
    private readonly ILogger<MarkSaleAsShippedCommandHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;

    public MarkSaleAsShippedCommandHandler(ILogger<MarkSaleAsShippedCommandHandler> logger, ISaleRepository<Sale> saleRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
    }
    
    public async Task<Result<SaleResult>> Handle(MarkSaleAsShippedCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);

        if (sale == null)
        {
            return Result<SaleResult>.Failure(new NotFoundError(request.SaleId, "Sale not found."));
        }

        if (sale.SellerId != request.UserId)
        {
            return Result<SaleResult>.Failure(new Forbidden("You are not allowed to mark this sale as shipped."));
        }
        
        // Domain
        sale.MarkDeliveryAsShipped();
        sale.GenerateDeliveryCode();
        
        // Persistence
        await _saleRepository.UpdateAsync(sale);
        
        _logger.LogInformation("Sale marked as shipped and generated delivery code");
        return Result<SaleResult>.Success(sale.ToSaleResult());
    }
}
