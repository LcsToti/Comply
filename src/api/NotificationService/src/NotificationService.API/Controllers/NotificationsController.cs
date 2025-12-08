using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.App.Queries.GetNotificationByUserId;
using NotificationService.App.UseCases.MarkNotificationAsRead;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var query = new GetNotificationsQuery { UserId = userId, Page = page, PageSize = pageSize };
        var result = await _mediator.Send(query);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new MarkAsReadCommand { NotificationId = id, UserId = userId };
        var result = await _mediator.Send(command);

        if (result.Status == App.Results.ResultStatus.NotFound)
        {
            return NotFound(result.Error);
        }

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}