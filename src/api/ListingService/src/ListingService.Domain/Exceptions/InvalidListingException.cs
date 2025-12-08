using ListingService.Domain.Common;

namespace ListingService.Domain.Exceptions;

public class InvalidListingException(string message) : DomainException(message);