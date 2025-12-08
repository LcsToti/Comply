using MediatR;
using SalesService.App.Common;
using SalesService.App.Common.Results;
using SalesService.App.Common.Results.Mappers;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;

namespace SalesService.App.Queries.SaleQueries.GetAllSales;

public class GetAllSalesQueryHandler(ISaleRepository<Sale> saleRepository) : IRequestHandler<GetAllSalesQuery, Result<List<SaleResult>>>
{
    private readonly ISaleRepository<Sale> _saleRepository = saleRepository;

    public async Task<Result<List<SaleResult>>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync();

        return Result<List<SaleResult>>.Success(sales.Select(s => s.ToSaleResult()).ToList());
    }
}