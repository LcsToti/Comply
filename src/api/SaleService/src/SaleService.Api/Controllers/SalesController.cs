using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesService.API.Common;
using SalesService.API.Common.Mappers;
using SalesService.API.Requests;
using SalesService.App.Commands.SaleCommands.Sales.CancelSale;
using SalesService.App.Common;
using SalesService.App.Common.Results;
using SalesService.App.Queries.SaleQueries.GetAllSales;
using SalesService.App.Queries.SaleQueries.GetAllSalesByUser;
using SalesService.App.Queries.SaleQueries.GetSaleDetails;

namespace SalesService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SalesController : ApiControllerBase<DeliveriesController>
{
    private readonly IMediator _mediator;
    
    public SalesController(ILogger<DeliveriesController> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;       
    }
    
    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost("{saleId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OpenDispute(Guid saleId)
    {
        if (string.IsNullOrEmpty(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            return Unauthorized();
        
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role != "Admin" && role != "Moderator")
            return Unauthorized();
        
        var command = new CancelSaleCommand(saleId, role);

        Result<SaleResult> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.ToSaleResponse()));
    }
    
    [Authorize(Roles = "Admin,Moderator")]
    [HttpGet]
    public async Task<IActionResult> GetAllSales()
    {
        var command = new GetAllSalesQuery();

        var result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.Select(v => v.ToSaleResponse())));
    }
    
    [HttpGet("{saleId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSaleById(Guid saleId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == null)
        {
            return Unauthorized();       
        }

        var command = new GetSaleDetailsQuery(saleId, userId, role);

        Result<SaleResult> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.ToSaleResponse()));
    }
    
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMySales([FromQuery] PagedRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new GetMySalesQuery(userId, request.Page, request.PageSize);

        Result<IEnumerable<SaleResult>> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.Select(v => v.ToSaleResponse())));
    }
}