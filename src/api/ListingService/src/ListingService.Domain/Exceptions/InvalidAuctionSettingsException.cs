using ListingService.Domain.Common;

namespace ListingService.Domain.Exceptions;

public class InvalidAuctionSettingsException(string message) : DomainException(message);