namespace ProductService.Application.Responses;

public record PaginatedList<T>(
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    List<T> Items)
    {
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }