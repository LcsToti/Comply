using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common;
using SalesService.App.Common.Errors;
using SalesService.App.Common.Results;
using SalesService.App.Common.Results.Mappers;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;

namespace SalesService.App.Queries.SaleQueries.GetSaleDetails;

public class GetSaleDetailsQueryHandler : IRequestHandler<GetSaleDetailsQuery, Result<SaleResult>>
{
    private readonly ILogger<GetSaleDetailsQueryHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    
    public GetSaleDetailsQueryHandler(ILogger<GetSaleDetailsQueryHandler> logger, ISaleRepository<Sale> saleRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
    }
    
    public async Task<Result<SaleResult>> Handle(GetSaleDetailsQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId);

        if (sale == null)
        {
            return Result<SaleResult>.Failure(new NotFoundError(request.SaleId, "Sale not found."));
        }

        if (request.Role is "Admin" or "Moderator")
        {
            return Result<SaleResult>.Success(sale.ToSaleResult());       
        }

        if (request.UserId != sale.SellerId && request.UserId != sale.BuyerId)
        {
            return Result<SaleResult>.Failure(new Forbidden("You are not allowed to get this sale details."));
        }

        return Result<SaleResult>.Success(sale.ToSaleResult());
    }
}