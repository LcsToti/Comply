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

namespace SalesService.App.Commands.SaleCommands.Dispute.CloseDispute;

public class CloseDisputeCommandHandler : IRequestHandler<CloseDisputeCommand, Result<SaleResult>>
{
    private readonly ILogger<CloseDisputeCommandHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    private readonly ISalePublisher _salePublisher;
    
    public CloseDisputeCommandHandler(ILogger<CloseDisputeCommandHandler> logger, ISaleRepository<Sale> saleRepository, ISalePublisher salePublisher)
    {
        _logger = logger;
        _saleRepository = saleRepository;
        _salePublisher = salePublisher;
    }
    
    public async Task<Result<SaleResult>> Handle(CloseDisputeCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);
        if (sale == null)
        {
            return Result<SaleResult>.Failure(new NotFoundError(request.SaleId, "Sale not found."));
        }

        if (sale.Dispute == null)
        {
            return Result<SaleResult>.Failure(new NotFoundError(request.SaleId, "Dispute not found."));      
        }

        if (sale.Status != SaleStatus.Dispute)
        {
            return Result<SaleResult>.Failure(new InvalidSaleOperation("Sale is not in dispute status."));      
        }

        if (sale.Dispute.AdminId != request.UserId)
        {
            return Result<SaleResult>.Failure(new Forbidden("You are not allowed to close this dispute."));       
        }

        if (request.Role != "Admin" && request.Role != "Moderator")
        {
            return Result<SaleResult>.Failure(new Forbidden("You are not allowed to close this dispute."));
        }
        
        //Domain
        sale.CloseDisputeAs(request.Resolution, request.ResolutionStatus);
        
        if (request.ResolutionStatus != DisputeResolutionStatus.Unsolved && request.ResolutionStatus != DisputeResolutionStatus.Expired)
        {
            sale.MarkAsDone();
        }
        
        //Persistence
        await _saleRepository.UpdateAsync(sale);
        
        //Publish
        if (sale.Dispute?.ResolutionStatus == DisputeResolutionStatus.Refunded)
        {
            await _salePublisher.PublishSaleRefundAsync(sale.PaymentId, sale.ProductValue, sale.Dispute.Reason, sale.BuyerId);
        }

        if (sale.Dispute?.ResolutionStatus == DisputeResolutionStatus.ApprovedWithdrawal)
        {
            await _salePublisher.PublishSaleDeliveredAsync(sale.PaymentId, sale.SellerId);
        }
        
        _logger.LogInformation("Dispute closed as {ResolutionStatus}", request.ResolutionStatus.ToString());
        return Result<SaleResult>.Success(sale.ToSaleResult());
    }
}