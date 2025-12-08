using ListingService.Api.Common;
using ListingService.Api.Common.Mappers;
using ListingService.App.Commands.AuctionCommands.Cancel;
using ListingService.App.Commands.AuctionCommands.Create;
using ListingService.App.Commands.AuctionCommands.EditSettings;
using ListingService.App.Commands.AuctionCommands.PrepareNewBid;
using ListingService.App.Common;
using ListingService.App.Common.Results;
using ListingService.App.Queries.AuctionQueries.GetAuctionBids;
using ListingService.App.Queries.AuctionQueries.GetById;
using ListingService.App.Queries.AuctionQueries.GetFiltered;
using ListingService.Contracts.Requests.Auctions;
using ListingService.Contracts.Responses.Auctions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ListingService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class AuctionsController(ISender sender, ILogger<AuctionsController> logger) : ApiControllerBase<AuctionsController>(logger)
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [ProducesResponseType<AuctionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new CreateAuctionCommand(
            UserId: userId,
            ListingId: request.ListingId,
            StartBidValue: request.StartBidValue,
            WinBidValue: request.WinBidValue,
            StartDate: request.StartDate,
            EndDate: request.EndDate);

        Result<AuctionResult> result = await _sender.Send(command);

        return HandleResult(result, value => CreatedAtAction(
            nameof(GetAuctionById),
            new { auctionId = value.Id },
            value.ToAuctionResponse()));
    }

    [HttpPut("{auctionId:guid}/settings")]
    [ProducesResponseType<AuctionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditAuctionSettings(Guid auctionId, [FromBody] EditAuctionSettingsRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new EditSettingsCommand(
            UserId: userId,
            AuctionId: auctionId,
            StartBidValue: request.StartBidValue,
            WinBidValue: request.WinBidValue,
            StartDate: request.StartDate,
            EndDate: request.EndDate);

        Result<AuctionResult> result = await _sender.Send(command);

        return HandleResult(result, value => Ok(value.ToAuctionResponse()));
    }

    [HttpPatch("{auctionId:guid}/cancel")]
    [ProducesResponseType<AuctionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelAuction(Guid auctionId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new CancelAuctionCommand(
           UserId: userId,
           AuctionId: auctionId);

        Result<AuctionResult> result = await _sender.Send(command);

        return HandleResult(result, value => Ok(value.ToAuctionResponse()));
    }

    [HttpPost("{auctionId:guid}/bids")]
    [ProducesResponseType<AuctionResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PrepareNewBid(Guid auctionId, [FromBody] PlaceNewBidRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new PrepareNewBidCommand(
            UserId: userId,
            AuctionId: auctionId,
            BidValue: request.Value,
            PaymentMethod: request.PaymentMethodId);

        Result<AuctionResult> result = await _sender.Send(command);

        return HandleResult(result, value => Accepted());
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<PagedList<AuctionResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuctions([FromQuery] GetAuctionsRequest request)
    {
        var query = new GetFilteredAuctionsQuery(
            request.MaxStartBid,
            request.StartsBefore,
            request.EndsBefore,
            request.Page,
            request.PageSize);

        Result<PagedList<AuctionResult>> result = await _sender.Send(query);

        return HandleResult(result, pagedResult =>
        {
            var responseItems = pagedResult.Items.Select(a => a.ToAuctionResponse()).ToList();
            var pagedResponse = new PagedList<AuctionResponse>(
                responseItems,
                pagedResult.Page,
                pagedResult.PageSize,
                pagedResult.TotalCount);
            return Ok(pagedResponse);
        });
    }

    [HttpGet("{auctionId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType<AuctionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAuctionById(Guid auctionId)
    {
        var query = new GetAuctionByIdQuery(auctionId);

        Result<AuctionResult> result = await _sender.Send(query);

        return HandleResult(result, value => Ok(value.ToAuctionResponse()));
    }

    [HttpGet("{auctionId:guid}/bids")]
    [AllowAnonymous]
    [ProducesResponseType<List<BidResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAuctionBids(Guid auctionId)
    {
        var query = new GetAuctionBidsQuery(auctionId);

        Result<List<BidResult>> result = await _sender.Send(query);

        return HandleResult(result, bids =>
        {
            var response = bids.Select(b => b.ToBidResponse()).ToList();
            return Ok(response);
        });
    }
}
