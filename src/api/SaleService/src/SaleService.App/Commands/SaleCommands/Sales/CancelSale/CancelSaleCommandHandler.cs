using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common;
using SalesService.App.Common.Errors;
using SalesService.App.Common.Results;
using SalesService.App.Common.Results.Mappers;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;

namespace SalesService.App.Commands.SaleCommands.Sales.CancelSale;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, Result<SaleResult>>
{
    private readonly ILogger<CancelSaleCommandHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    public CancelSaleCommandHandler(ILogger<CancelSaleCommandHandler> logger, ISaleRepository<Sale> saleRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
    }
    
    public async Task<Result<SaleResult>> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);
        if (sale == null)
        {
            return Result<SaleResult>.Failure(new NotFoundError(request.SaleId, "Sale not found."));
        }

        if (request.Role != "Admin" && request.Role != "Moderator")
        {
            return Result<SaleResult>.Failure(new Forbidden("You are not allowed to cancel this sale."));
        }
        
        //Domain
        sale.MarkAsCancelled();
        
        //Persistence
        await _saleRepository.UpdateAsync(sale);
        
        _logger.LogInformation("Sale cancelled");
        return Result<SaleResult>.Success(sale.ToSaleResult());
    }
}