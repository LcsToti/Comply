using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.ImagesCommands.AddImages;
using ProductService.Application.Commands.ImagesCommands.RemoveImage;
using ProductService.Application.Commands.ImagesCommands.ReorderImages;
using ProductService.Application.Commands.ProductsCommands.AddFeature;
using ProductService.Application.Commands.ProductsCommands.CreateProduct;
using ProductService.Application.Commands.ProductsCommands.UpdateProduct;
using ProductService.Application.Queries.ProductsQueries.GetById;
using ProductService.Application.Queries.ProductsQueries.GetFiltered;
using ProductService.Application.Queries.ProductsQueries.GetMyBiddedProducts;
using ProductService.Application.Queries.ProductsQueries.GetMyBoughtProducts;
using ProductService.Application.Queries.ProductsQueries.GetMyListedProducts;
using ProductService.Application.Queries.ProductsQueries.GetMyOutbidProducts;
using ProductService.Application.Queries.ProductsQueries.GetMyWinningProducts;
using ProductService.Application.Responses;
using System.Security.Claims;
using ProductService.API.Requests.Images;
using ProductService.API.Requests.Products;
using ProductService.Application.Queries.ProductsQueries.GetActiveAuctionsCount;
using ProductService.Application.Queries.ProductsQueries.GetProductsCount;

namespace ProductService.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    #region Commands
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request, [FromQuery] bool isTest = false)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role is null)
            return Unauthorized("User role not found.");
        if (role == "User") isTest = false;

        var command = new CreateProductCommand(
            userId,
            request.Title,
            request.Description,
            request.ImageUrls,
            request.Locale,
            request.Characteristics,
            request.Condition,
            request.Category,
            request.DeliveryPreference,
            isTest,
            role);

        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProductById), new { productId = response.Id }, response);
    }

    [HttpPatch("{productId:guid}")]
    public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductRequest request, Guid productId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new UpdateProductCommand(
            productId,
            userId,
            request.Title,
            request.Description,
            request.Locale,
            request.Characteristics,
            request.Condition,
            request.Category,
            request.DeliveryPreference);

        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("{productId:guid}/images")]
    public async Task<IActionResult> AddImages(Guid productId, [FromForm] ImagesRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new AddImagesCommand(productId, userId, request.ImageUrls);

        await _mediator.Send(command);

        return Ok();
    }

    [HttpPut("{productId:guid}/images")]
    public async Task<IActionResult> ReorderImages(Guid productId, [FromBody] ImagesUrlsRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new ReorderImagesCommand(productId, userId, request.ImageUrls);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpDelete("{productId:guid}/images")]
    public async Task<IActionResult> RemoveImage(Guid productId, [FromBody] ImagesUrlsRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new RemoveImageCommand(productId, userId, request.ImageUrls);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("{productId:guid}/feature")]
    public async Task<IActionResult> AddFeature(Guid productId, [FromBody] int durationInDays)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var command = new AddFeatureCommand(productId, userId, durationInDays);
        await _mediator.Send(command);

        return Ok();
    }
    #endregion

    #region Queries
    [HttpGet("{productId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById(Guid productId)
    {
        var command = new GetProductByIdQuery(productId);

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedList<ProductResponse>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetFilteredProducts(
        [FromQuery] GetFilteredProductsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
    
    [HttpGet("count")]
    [Authorize(Roles = "Admin, Moderator")]
    public async Task<IActionResult> GetProductsCount()
    {
        var query = new GetProductsCountQuery();
        
        var result = await _mediator.Send(query);

        return Ok(result);
    }
    
    [HttpGet("ActiveAuctions/count")]
    [Authorize(Roles = "Admin, Moderator")]
    public async Task<IActionResult> GetActiveAuctionsCount()
    {
        var query = new GetActiveAuctionsCountQuery();
        
        var result = await _mediator.Send(query);

        return Ok(result);
    }
    #endregion

    #region Me Queries
    [HttpGet("me/listed")]
    public async Task<IActionResult> GetMyListedProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("PageNumber and PageSize should be greater than 0.");

        var query = new GetMyListedProductsQuery(userId, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("me/bidded")]
    public async Task<IActionResult> GetMyBiddedProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("PageNumber and PageSize should be greater than 0.");

        var query = new GetMyBiddedProductsQuery(userId, pageNumber, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("me/outbid")]
    public async Task<IActionResult> GetMyOutbidProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("PageNumber and PageSize should be greater than 0.");

        var query = new GetMyOutbidProductsQuery(userId, pageNumber, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("me/winning")]
    public async Task<IActionResult> GetMyWinningProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("PageNumber and PageSize should be greater than 0.");

        var query = new GetMyWinningProductsQuery(userId, pageNumber, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("me/bought")]
    public async Task<IActionResult> GetBoughtProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("PageNumber and PageSize should be greater than 0.");

        var query = new GetMyBoughtProductsQuery(userId, pageNumber, pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
    #endregion
}