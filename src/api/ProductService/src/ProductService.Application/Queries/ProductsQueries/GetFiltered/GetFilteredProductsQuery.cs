using MediatR;
using ProductService.Application.Responses;
using ProductService.Domain.Enums;
using System.Text.Json.Serialization;

namespace ProductService.Application.Queries.ProductsQueries.GetFiltered;

public record GetFilteredProductsQuery : IRequest<PaginatedList<ProductResponse>>
{
    // Pagination
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    // Filters
    public string? SearchTerm { get; init; }
    
    public Guid? SellerId { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Categories? Category { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductCondition? Condition { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public string? ListingStatus { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public string? AuctionStatus { get; init; }

    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }

    public bool OnlyAuctionWithoutBids { get; init; } = false;
    
    public decimal? MinBidValue { get; init; }
    public decimal? MaxBidValue { get; init; }

    // Sorting
    public ProductSortBy? SortBy { get; init; } = ProductSortBy.Newest;
}

public enum ProductSortBy
{
    Newest,
    Oldest,
    MostBids,
    LessBids,
    Popularity,
    PriceAsc,
    PriceDesc,
    AuctionStartingSoon,
    AuctionEndingSoon
}