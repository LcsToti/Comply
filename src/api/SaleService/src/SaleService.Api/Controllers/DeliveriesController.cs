using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesService.API.Common;
using SalesService.API.Common.Mappers;
using SalesService.API.Requests;
using SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsDelivered;
using SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsShipped;
using SalesService.App.Common;
using SalesService.App.Common.Results;
using SalesService.App.Queries.SaleQueries.GetSaleDeliveryCode;

namespace SalesService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]/{saleId:guid}")]
[Authorize]
public class DeliveriesController : ApiControllerBase<DeliveriesController>
{
    private readonly IMediator _mediator;
    
    public DeliveriesController(ILogger<DeliveriesController> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;       
    }

    [HttpGet("delivery-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeliveryCode(Guid saleId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new GetSaleDeliveryCodeQuery(saleId, userId);

        Result<string> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value));
    }
    
    [HttpPost("delivered")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkSaleAsDelivered(Guid saleId, [FromBody] DeliveryCodeRequest request)
    {
        if (string.IsNullOrEmpty(User.FindFirstValue(ClaimTypes.NameIdentifier)))
            return Unauthorized();

        var command = new MarkSaleAsDeliveredCommand(saleId, request.Code);

        Result<SaleResult> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.ToSaleResponse()));
    }
    
    [HttpPost("shipped")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkSaleAsShipped(Guid saleId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new MarkSaleAsShippedCommand(saleId, userId);

        Result<SaleResult> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.ToSaleResponse()));
    }
}