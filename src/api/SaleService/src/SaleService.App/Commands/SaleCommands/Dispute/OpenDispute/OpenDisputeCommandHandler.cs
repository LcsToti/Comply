using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common;
using SalesService.App.Common.Contracts;
using SalesService.App.Common.Errors;
using SalesService.App.Common.Results;
using SalesService.App.Common.Results.Mappers;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;

namespace SalesService.App.Commands.SaleCommands.Dispute.OpenDispute;

public class OpenDisputeCommandHandler : IRequestHandler<OpenDisputeCommand, Result<SaleResult>>
{
    private readonly ILogger<OpenDisputeCommandHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    private readonly ISalePublisher _salePublisher;
    
    public OpenDisputeCommandHandler(ILogger<OpenDisputeCommandHandler> logger, ISaleRepository<Sale> saleRepository, ISalePublisher salePublisher)
    {
        _logger = logger;
        _saleRepository = saleRepository;
        _salePublisher = salePublisher;
    }
    
    public async Task<Result<SaleResult>> Handle(OpenDisputeCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);
        if (sale == null)
        {
            return Result<SaleResult>.Failure(new NotFoundError(request.SaleId, "Sale not found."));
        }

        if (request.UserId != sale.BuyerId)
        {
            return Result<SaleResult>.Failure(new Forbidden("You are not allowed to open this dispute."));       
        }
        
        //Domain
        sale.OpenDispute(request.Reason);
        
        //Persistence
        await _saleRepository.UpdateAsync(sale);
        
        //Publish
        await _salePublisher.PublishDisputeExpirationDateAsync(sale.Id, TimeSpan.FromDays(30));
        
        _logger.LogInformation("Dispute opened");
        return Result<SaleResult>.Success(sale.ToSaleResult());
    }
}