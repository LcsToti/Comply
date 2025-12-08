using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.API.Requests;
using NotificationService.App.Commands.Tickets.AddCommentToTicket;
using NotificationService.App.Commands.Tickets.AssignTicketToAdmin;
using NotificationService.App.Commands.Tickets.CreateSupportTicket;
using NotificationService.App.Queries.GetAllTickets;
using NotificationService.App.Queries.GetTicketByUserId;
using NotificationService.App.Results;
using NotificationService.App.UseCases.GetTicketById;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "Admin, Moderator")]
    [HttpGet]
    public async Task<IActionResult> GetAllTickets()
    {
        var query = new GetAllTicketsQuery(); // todo: Pagination
        
        var result = await _mediator.Send(query);
        
        return Ok(result.Value);
    }

    [HttpGet("{id}", Name = nameof(GetTicketById))]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        var query = new GetTicketByIdQuery(id);

        var result = await _mediator.Send(query);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(result.Error);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyTickets()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var query = new GetTicketByUserIdQuery(userId);

        var result = await _mediator.Send(query);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest command)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new CreateTicketCommand(userId, command.Title, command.Description));

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTicketById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{ticketId:guid}/status")]
    public async Task<IActionResult> UpdateTicketStatus(Guid ticketId, [FromBody] AssignTicketRequest command)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var result = await _mediator.Send(new AssignTicketCommand(ticketId, userId, command.NewStatus));

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{ticketId:guid}/comment")]
    public async Task<IActionResult> AddComment(Guid ticketId, [FromBody] AddCommentToTicketRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();


        var result = await _mediator.Send(new AddCommentToTicketCommand(ticketId, userId, request.Content));

        if (result.Status == ResultStatus.NotFound)
        {
            return NotFound(result.Error);
        }

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}