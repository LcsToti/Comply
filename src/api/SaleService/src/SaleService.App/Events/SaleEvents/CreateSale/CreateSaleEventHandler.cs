using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common.Contracts;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Aggregates.SaleAggregate.Factories;
using SalesService.Domain.Contracts;

namespace SalesService.App.Events.SaleEvents.CreateSale;

public class CreateSaleEventHandler : IRequestHandler<CreateSaleEvent>
{
    private readonly ILogger<CreateSaleEventHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;

    public CreateSaleEventHandler(ILogger<CreateSaleEventHandler> logger, ISaleRepository<Sale> saleRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
    }
    
    public async Task Handle(CreateSaleEvent request, CancellationToken cancellationToken)
    {
        // Domain
        Sale sale = SaleFactory.Create(request.ProductId, request.BuyerId, request.SellerId, request.ListingId, request.PaymentId, request.ProductValue);
        
        // Persist
        _logger.LogInformation("Sale created on mongo");
        await _saleRepository.AddAsync(sale);
    }
}