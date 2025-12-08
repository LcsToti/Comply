using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Aggregates.SaleAggregate.Enums;
using SalesService.Domain.Contracts;

namespace SalesService.App.Events.SaleEvents.CloseDisputeAsExpired;

public class CloseDisputeAsExpiredEventHandler : IRequestHandler<CloseDisputeAsExpiredEvent>
{
    private readonly ILogger<CloseDisputeAsExpiredEventHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    
    public CloseDisputeAsExpiredEventHandler(ILogger<CloseDisputeAsExpiredEventHandler> logger, ISaleRepository<Sale> saleRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
    }
    
    public async Task Handle(CloseDisputeAsExpiredEvent request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);
        if (sale == null)
        {
            _logger.LogInformation("Sale not found");
            return;       
        }

        if (sale.Dispute == null)
        {
            _logger.LogInformation("Sale does not have a dispute");
            return;      
        }
        
        var elapsed = DateTime.UtcNow - sale.Dispute.CreatedAt;
        
        if (elapsed >= request.ExpiresAt)
        {
            _logger.LogInformation("Dispute expired");
            //Domain
            sale.CloseDisputeAs("Disputa encerrada por tempo limite(30 dias)..", DisputeResolutionStatus.Expired);
            
            //Persistence       
            await _saleRepository.UpdateAsync(sale);
        }
    }
}