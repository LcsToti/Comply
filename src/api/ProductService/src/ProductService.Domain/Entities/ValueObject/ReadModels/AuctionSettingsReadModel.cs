namespace ProductService.Domain.Entities.ValueObject.ReadModels;

public record AuctionSettingsReadModel(
    decimal StartBidValue,
    decimal WinBidValue,
    DateTime StartDate,
    DateTime EndDate);