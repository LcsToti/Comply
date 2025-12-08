namespace ProductService.Application.Responses;

public record ProductResponse(
    Guid Id,
    Guid SellerId,
    string Title,
    string Description,
    string Locale,
    List<string> Images,
    Dictionary<string, string> Characteristics,
    string Condition,
    string Category,
    string DeliveryPreference,
    int WatchListCount,
    bool Featured,
    DateTime? ExpirationFeatureDate,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    ListingReadModelResponse? Listing,
    QnaResponse Qna);

#region ReadModel DTOs
public record ListingReadModelResponse(
    Guid Id,
    string Status,
    decimal BuyPrice,
    bool IsAuctionActive,
    bool IsProcessingPurchase,
    Guid? BuyerId,
    Guid? AuctionId,
    DateTime ListedAt,
    DateTime UpdatedAt,
    AuctionReadModelResponse? Auction);

public record AuctionReadModelResponse(
    Guid Id,
    string Status,
    bool IsProcessingBid,
    DateTime? EditedAt,
    DateTime? StartedAt,
    DateTime? EndedAt,
    AuctionSettingsReadModelResponse Settings,
    List<BidReadModelResponse> Bids);

public record BidReadModelResponse(
    Guid Id,
    Guid BidderId,
    decimal Value,
    string Status,
    DateTime BiddedAt,
    DateTime? OutbiddedAt,
    DateTime? WonAt);

public record AuctionSettingsReadModelResponse(
    decimal StartBidValue,
    decimal WinBidValue,
    DateTime StartDate,
    DateTime EndDate);
#endregion

#region Qna DTOs
public record QnaResponse(
    int TotalQuestions,
    List<QuestionResponse> Questions);

public record QuestionResponse(
    Guid QuestionId,
    Guid UserId,
    string QuestionText,
    DateTime AskedAt,
    AnswerResponse? Answer);

public record AnswerResponse(
    string AnswerText,
    DateTime AnsweredAt);
#endregion