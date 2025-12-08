using ListingService.Domain.Common;

namespace ListingService.Domain.Exceptions;

public class InvalidAuctionException(string message) : DomainException(message);