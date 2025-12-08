using ListingService.Domain.Common;

namespace ListingService.Domain.Exceptions;

public class InvalidBidException(string message) : DomainException(message);