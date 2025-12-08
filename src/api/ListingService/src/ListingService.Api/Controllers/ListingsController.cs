using ListingService.Api.Common;
using ListingService.Api.Common.Mappers;
using ListingService.App.Commands.ListingCommands.Create;
using ListingService.App.Commands.ListingCommands.PrepareBuyNow;
using ListingService.App.Commands.ListingCommands.ToggleAvailability;
using ListingService.App.Commands.ListingCommands.UpdateBuyPrice;
using ListingService.App.Common;
using ListingService.App.Common.Results;
using ListingService.App.Queries.ListingQueries.GetById;
using ListingService.App.Queries.ListingQueries.GetFiltered;
using ListingService.Contracts.Requests.Listings;
using ListingService.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ListingService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class ListingsController(
    ISender sender,
    ILogger<ListingsController> logger) : ApiControllerBase<ListingsController>(logger)
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [ProducesResponseType<ListingResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateListing([FromBody] CreateListingRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new CreateListingCommand(
            UserId: userId,
            ProductId: request.ProductId,
            BuyPrice: request.BuyPrice);

        Result<ListingResult> result = await _sender.Send(command);

        return HandleResult(result, value => CreatedAtAction(
            nameof(GetListingById),
            new { listingId = value.Id },
            value.ToListingResponse()));
    }

    [HttpPatch("{listingId:guid}/price")]
    [ProducesResponseType<ListingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBuyPrice(Guid listingId, [FromBody] UpdateBuyPriceRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new UpdateBuyPriceCommand(
            UserId: userId,
            ListingId: listingId,
            NewBuyPrice: request.NewBuyPrice);

        Result<ListingResult> result = await _sender.Send(command);

        return HandleResult(result, value => Ok(value.ToListingResponse()));
    }

    [HttpPatch("{listingId:guid}/availability")]
    [ProducesResponseType<ListingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleAvailability(Guid listingId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new ToggleAvailabilityCommand(
            UserId: userId,
            ListingId: listingId);

        Result<ListingResult> result = await _sender.Send(command);

        return HandleResult(result, value => Ok(value.ToListingResponse()));
    }


    [HttpPatch("{listingId:guid}/buynow")]
    [ProducesResponseType<ListingResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PrepareBuyNow(Guid listingId, BuyNowRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new PrepareBuyNowCommand(
            UserId: userId,
            ListingId: listingId,
            PaymentMethod: request.PaymentMethod);

        Result<ListingResult> result = await _sender.Send(command);

        return HandleResult(result, value => Accepted());
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<PagedList<ListingResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListings([FromQuery] GetListingsRequest request)
    {
        var query = new GetFilteredListingsQuery(
            request.MinBuyPrice,
            request.MaxBuyPrice,
            request.Status,
            request.SellerId,
            request.Page,
            request.PageSize);

        Result<PagedList<ListingResult>> result = await _sender.Send(query);

        return HandleResult(result, pagedResult =>
        {
            var responseItems = pagedResult.Items.Select(l => l.ToListingResponse()).ToList();
            var pagedResponse = new PagedList<ListingResponse>(
                responseItems,
                pagedResult.Page,
                pagedResult.PageSize,
                pagedResult.TotalCount);
            return Ok(pagedResponse);
        });
    }

    [HttpGet("{listingId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType<ListingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetListingById(Guid listingId)
    {
        var query = new GetListingByIdQuery(listingId);

        Result<ListingResult> result = await _sender.Send(query);

        return HandleResult(result, value => Ok(value.ToListingResponse()));
    }
}