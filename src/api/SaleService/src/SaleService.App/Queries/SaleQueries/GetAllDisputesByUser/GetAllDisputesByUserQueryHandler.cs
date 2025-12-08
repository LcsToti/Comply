using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Common;
using SalesService.App.Common.Errors;
using SalesService.App.Common.Results;
using SalesService.App.Common.Results.Mappers;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;

namespace SalesService.App.Queries.SaleQueries.GetAllDisputesByUser;

public class GetAllDisputesByUserQueryHandler : IRequestHandler<GetAllDisputesByUserQuery, Result<IEnumerable<SaleResult>>>
{
    private readonly ILogger<GetAllDisputesByUserQueryHandler> _logger;
    private readonly ISaleRepository<Sale> _saleRepository;
    
    public GetAllDisputesByUserQueryHandler(ILogger<GetAllDisputesByUserQueryHandler> logger, ISaleRepository<Sale> saleRepository)
    {
        _logger = logger;
        _saleRepository = saleRepository;
    }
    
    public async Task<Result<IEnumerable<SaleResult>>> Handle(GetAllDisputesByUserQuery request, CancellationToken cancellationToken)
    {
        var salesWithDispute = await _saleRepository.GetAllDisputesByUserPagedAsync(request.UserId, request.Page, request.PageSize);
        
        if (salesWithDispute == null)
        {
            return Result<IEnumerable<SaleResult>>.Failure(new NotFoundError(request.UserId, "Disputes not found for this user."));
        }
        
        return Result<IEnumerable<SaleResult>>.Success(salesWithDispute.Select(swd => swd.ToSaleResult()));
    }
}