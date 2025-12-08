using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common;
using SalesService.App.Common.Errors;
using SalesService.App.Common.Results;
using SalesService.App.Common.Results.Mappers;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;

namespace SalesService.App.Queries.SaleQueries.GetAllSalesByUser;

public class GetMySalesQueryHandler : IRequestHandler<GetMySalesQuery, Result<IEnumerable<SaleResult>>>
{
    private readonly ILogger<GetMySalesQueryHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    
    public GetMySalesQueryHandler(ISaleRepository<Sale> saleRepository, ILogger<GetMySalesQueryHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }
    
    public async Task<Result<IEnumerable<SaleResult>>> Handle(GetMySalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllByUserPagedAsync(request.UserId, request.Page, request.PageSize);

        if (sales == null)
        {
            return Result<IEnumerable<SaleResult>>.Failure(new NotFoundError(request.UserId, "Sales not found for this user."));
        }
        
        return Result<IEnumerable<SaleResult>>.Success(sales.Select(s => s.ToSaleResult()));
    }
}