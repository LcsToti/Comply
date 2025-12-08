using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.App.Commands.WatchList.AddToWatchList;
using NotificationService.App.Commands.WatchList.GetWatchList;
using NotificationService.App.Commands.WatchList.RemoveFromWatchList;
using NotificationService.API.Requests;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class WatchListController : ControllerBase
{
    private readonly IMediator _mediator;

    public WatchListController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMyWatchlist()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = await _mediator.Send(new GetWatchListCommand(userId));
        return Ok(command);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddToWatchlist([FromBody] AddToWatchListRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        await _mediator.Send(new AddToWatchListCommand(userId, request.ProductId, request.ListingId));
        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveFromWatchlist([FromBody] RemoveFromWatchListRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        await _mediator.Send(new RemoveFromWatchListCommand(userId, request.ProductId));
        return Ok();
    }
}