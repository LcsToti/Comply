using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common;
using SalesService.App.Common.Contracts;
using SalesService.App.Common.Errors;
using SalesService.App.Common.Results;
using SalesService.App.Common.Results.Mappers;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Aggregates.SaleAggregate.Enums;
using SalesService.Domain.Contracts;

namespace SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsDelivered;

public class MarkSaleAsDeliveredCommandHandler : IRequestHandler<MarkSaleAsDeliveredCommand, Result<SaleResult>>
{
    private readonly ILogger<MarkSaleAsDeliveredCommandHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    private readonly ISalePublisher _salePublisher;
    
    public MarkSaleAsDeliveredCommandHandler(ILogger<MarkSaleAsDeliveredCommandHandler> logger, ISaleRepository<Sale> saleRepository, ISalePublisher salePublisher)
    {
        _logger = logger;
        _saleRepository = saleRepository;
        _salePublisher = salePublisher;
    }
    
    public async Task<Result<SaleResult>> Handle(MarkSaleAsDeliveredCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);

        if (sale == null)
        {
            return Result<SaleResult>.Failure(new NotFoundError(request.SaleId, "Sale not found."));
        }

        if (sale.Status == SaleStatus.Dispute)
        {
            return Result<SaleResult>.Failure(new InvalidSaleOperation("Sale is in dispute status, wait for the dispute to be resolved."));              
        }
        
        // Domain
        sale.MarkDeliveryAsDelivered(request.DeliveryCode);
        _logger.LogInformation("Delivery code {DeliveryCode} is valid", request.DeliveryCode);
        sale.MarkAsDone();
        
        // Persistence
        await _saleRepository.UpdateAsync(sale);
        
        // Publish
        await _salePublisher.PublishSaleDeliveredAsync(sale.PaymentId, sale.SellerId);
        
        _logger.LogInformation("Sale marked as delivered");
        return Result<SaleResult>.Success(sale.ToSaleResult());
    }
}