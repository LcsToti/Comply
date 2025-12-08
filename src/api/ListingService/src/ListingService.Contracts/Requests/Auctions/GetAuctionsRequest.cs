namespace ListingService.Contracts.Requests.Auctions;

public record GetAuctionsRequest(
decimal? MaxStartBid,
DateTime? StartsBefore,
DateTime? EndsBefore,
int Page = 1,
int PageSize = 20);