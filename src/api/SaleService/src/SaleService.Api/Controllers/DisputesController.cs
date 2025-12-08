using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesService.API.Common;
using SalesService.API.Common.Mappers;
using SalesService.API.Requests;
using SalesService.App.Commands.SaleCommands.Dispute.AssignAdminToDispute;
using SalesService.App.Commands.SaleCommands.Dispute.CloseDispute;
using SalesService.App.Commands.SaleCommands.Dispute.OpenDispute;
using SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsDelivered;
using SalesService.App.Commands.SaleCommands.Sales.MarkSaleAsShipped;
using SalesService.App.Common;
using SalesService.App.Common.Results;
using SalesService.App.Queries.SaleQueries.GetAllDisputesByUser;
using SalesService.App.Queries.SaleQueries.GetSaleDeliveryCode;

namespace SalesService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DisputesController : ApiControllerBase<DeliveriesController>
{
    private readonly IMediator _mediator;
    
    public DisputesController(ILogger<DeliveriesController> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;       
    }
    
    [HttpPost("{saleId:guid}/open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OpenDispute(Guid saleId, [FromBody] ReasonRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new OpenDisputeCommand(saleId, request.Reason, userId);

        Result<SaleResult> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.ToSaleResponse()));
    }
    
    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost("{saleId:guid}/close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseDispute(Guid saleId, [FromBody] CloseDisputeRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role == null)
            return Unauthorized();       

        var command = new CloseDisputeCommand(saleId, request.Resolution, role, request.ResolutionStatus, userId);

        Result<SaleResult> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.ToSaleResponse()));
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost("{saleId:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignAdminToDispute(Guid saleId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role != "Admin" && role != "Moderator")
            return Unauthorized();

        var command = new AssignAdminToDisputeCommand(saleId, role, userId);
        
        Result<SaleResult> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.ToSaleResponse()));
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllDisputesByUser([FromQuery] PagedRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var command = new GetAllDisputesByUserQuery(userId, request.Page, request.PageSize);

        Result<IEnumerable<SaleResult>> result = await _mediator.Send(command);

        return HandleResult(result, value => Ok(value.Select(v => v.ToSaleResponse())));
    }
}