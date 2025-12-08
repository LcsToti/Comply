using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common;
using SalesService.App.Common.Errors;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Aggregates.SaleAggregate.Enums;
using SalesService.Domain.Contracts;

namespace SalesService.App.Queries.SaleQueries.GetSaleDeliveryCode;

public class GetSaleDeliveryCodeQueryHandler : IRequestHandler<GetSaleDeliveryCodeQuery, Result<string>>
{
    private readonly ILogger<GetSaleDeliveryCodeQueryHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    
    public GetSaleDeliveryCodeQueryHandler(ILogger<GetSaleDeliveryCodeQueryHandler> logger, ISaleRepository<Sale> saleRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
    }

    public async Task<Result<string>> Handle(GetSaleDeliveryCodeQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);
        if (sale == null)
        {
            return Result<string>.Failure(new NotFoundError(request.SaleId, "Sale not found."));
        }

        if (sale.DeliveryStatus != DeliveryStatus.Shipped)
        {
            return Result<string>.Failure(new InvalidSaleOperation("Sale is not in shipped status."));       
        }
        
        if (sale.DeliveryCode == null)
        {
            return Result<string>.Failure(new NotFoundError(request.SaleId, "Sale delivery code not found."));       
        }

        if(request.UserId != sale.BuyerId)
        {
            return Result<string>.Failure(new Forbidden("You are not allowed to get this sale delivery code."));
        }

        return Result<string>.Success(sale.DeliveryCode);
    }
}