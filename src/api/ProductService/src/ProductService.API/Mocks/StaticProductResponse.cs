using ProductService.Application.Responses;

namespace ProductService.API.Mocks;

public static class StaticProductResponse
{
    private static readonly Guid MockSellerId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    private static readonly Guid MockProductId = Guid.Parse("c92c57f5-937b-402c-a81d-e59a111a68e8");

    public static ProductResponse Create()
    {
        var now = DateTime.UtcNow;
        var listingId = Guid.NewGuid();
        var auctionId = Guid.NewGuid();

        var auctionSettings = new AuctionSettingsReadModelResponse(
            StartBidValue: 100.00m,
            WinBidValue: 500.00m,
            StartDate: now.AddDays(-1),
            EndDate: now.AddDays(7)
        );

        var bids = new List<BidReadModelResponse>
        {
            new(
                Id: Guid.NewGuid(),
                BidderId: Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"),
                Value: 120.00m,
                Status: "Outbidded",
                BiddedAt: now.AddHours(-10),
                OutbiddedAt: now.AddHours(-1),
                WonAt: null),

            new(
                Id: Guid.NewGuid(),
                BidderId: Guid.Parse("f6e5d4c3-b2a1-0f9e-8d7c-6b5a4f3e2d1c"),
                Value: 150.00m,
                Status: "CurrentWinner",
                BiddedAt: now.AddHours(-1),
                OutbiddedAt: null,
                WonAt: null)
        };

        var auction = new AuctionReadModelResponse(
            Id: auctionId,
            Status: "Active",
            IsProcessingBid: false,
            EditedAt: null,
            StartedAt: now.AddDays(-1),
            EndedAt: now.AddDays(7),
            Settings: auctionSettings,
            Bids: bids
        );

        var listing = new ListingReadModelResponse(
            Id: listingId,
            Status: "Active",
            BuyPrice: 200.00m,
            IsAuctionActive: true,
            IsProcessingPurchase: false,
            BuyerId: null,
            AuctionId: auctionId,
            ListedAt: now.AddDays(-2),
            UpdatedAt: now,
            Auction: auction
        );

        var question1Answer = new AnswerResponse(
            AnswerText: "Sim, ele está na caixa original e nunca foi usado.",
            AnsweredAt: now.AddHours(-5)
        );

        var question1 = new QuestionResponse(
            QuestionId: Guid.NewGuid(),
            UserId: Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
            QuestionText: "O produto é novo e lacrado?",
            AskedAt: now.AddHours(-6),
            Answer: question1Answer
        );

        var question2 = new QuestionResponse(
            QuestionId: Guid.NewGuid(),
            UserId: Guid.Parse("9f8e7d6c-5b4a-3f2e-1d0c-9b8a7f6e5d4c"),
            QuestionText: "Qual o custo do frete para São Paulo?",
            AskedAt: now.AddHours(-2),
            Answer: null
        );

        var qna = new QnaResponse(
            TotalQuestions: 2,
            Questions: [question1, question2]
        );

        var productResponse = new ProductResponse(
            Id: MockProductId,
            SellerId: MockSellerId,
            Title: "Smartphone Ultra Power X",
            Description: "O mais novo smartphone com câmera de 108MP e bateria de longa duração. Estado impecável.",
            Locale: "São Paulo, SP, Brasil",
            Images:
            [
                "https://example.com/images/produto-x-1.jpg",
                "https://example.com/images/produto-x-2.jpg",
                "https://example.com/images/produto-x-3.jpg"
            ],
            Characteristics: new Dictionary<string, string>
            {
                { "Cor", "Preto Espacial" },
                { "Memória RAM", "12GB" },
                { "Armazenamento", "256GB" }
            },
            Condition: "New",
            Category: "Electronics",
            DeliveryPreference: "Both",
            WatchListCount: 42,
            Featured: true,
            ExpirationFeatureDate: now.AddDays(30),
            CreatedAt: now.AddMonths(-3),
            UpdatedAt: now,
            Listing: listing,
            Qna: qna
        );

        return productResponse;
    }
}