using ProductService.Domain.Entities;

namespace ProductService.Application.Responses;

public static class ProductMapper
{
    public static ProductResponse ToProductResponse(this Product product)
    {
        var listingResponse = product.Listing is null
            ? null
            : new ListingReadModelResponse(
                Id: product.Listing.Id,
                Status: product.Listing.Status,
                BuyPrice: product.Listing.BuyPrice,
                IsAuctionActive: product.Listing.IsAuctionActive,
                IsProcessingPurchase: product.Listing.IsProcessingPurchase,
                BuyerId: product.Listing.BuyerId,
                AuctionId: product.Listing.AuctionId,
                ListedAt: product.Listing.ListedAt,
                UpdatedAt: product.Listing.UpdatedAt,
                Auction: product.Listing.Auction is null
                    ? null
                    : new AuctionReadModelResponse(
                        Id: product.Listing.Auction.Id,
                        Status: product.Listing.Auction.Status,
                        IsProcessingBid: product.Listing.Auction.IsProcessingBid,
                        EditedAt: product.Listing.Auction.EditedAt,
                        StartedAt: product.Listing.Auction.StartedAt,
                        EndedAt: product.Listing.Auction.EndedAt,
                        Settings: new AuctionSettingsReadModelResponse(
                            StartBidValue: product.Listing.Auction.Settings.StartBidValue,
                            WinBidValue: product.Listing.Auction.Settings.WinBidValue,
                            StartDate: product.Listing.Auction.Settings.StartDate,
                            EndDate: product.Listing.Auction.Settings.EndDate
                        ),
                        Bids: [.. product.Listing.Auction.Bids.Select(b => new BidReadModelResponse(
                            Id: b.Id,
                            BidderId: b.BidderId,
                            Value: b.Value,
                            Status: b.Status,
                            BiddedAt: b.BiddedAt,
                            OutbiddedAt: b.OutbiddedAt,
                            WonAt: b.WonAt
                        ))]
                    )
            );

        var qnaResponse = product.Qna is null
            ? null
            : new QnaResponse(
                TotalQuestions: product.Qna.TotalQuestions,
                Questions: product.Qna.Questions?.Select(q => new QuestionResponse(
                    QuestionId: q.QuestionId,
                    UserId: q.UserId,
                    QuestionText: q.QuestionText,
                    AskedAt: q.AskedAt,
                    Answer: q.Answer is null
                        ? null
                        : new AnswerResponse(
                            AnswerText: q.Answer.AnswerText,
                            AnsweredAt: q.Answer.AnsweredAt
                        )
                )).ToList() ?? []
            );

        return new ProductResponse(
            Id: product.ProductId,
            SellerId: product.SellerId,
            Title: product.Title,
            Description: product.Description,
            Locale: product.Locale,
            Images: product.Images ?? [],
            Characteristics: product.Characteristics ?? [],
            Condition: product.Condition.ToString(),
            Category: product.Category.ToString(),
            DeliveryPreference: product.DeliveryPreference.ToString(),
            WatchListCount: product.WatchListCount,
            Featured: product.Featured,
            ExpirationFeatureDate: product.ExpirationFeatureDate,
            CreatedAt: product.CreatedAt,
            UpdatedAt: product.UpdatedAt,
            Listing: listingResponse,
            Qna: qnaResponse ?? new QnaResponse(0, [])
        );
    }
}
