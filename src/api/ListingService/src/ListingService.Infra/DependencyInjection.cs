using ListingService.App.Common.Interfaces;
using ListingService.Domain.Repositories;
using ListingService.Infra.Consumers.AuctionStatusConsumers;
using ListingService.Infra.Consumers.PaymentsMessagesConsumers;
using ListingService.Infra.Consumers.TimeoutConsumers;
using ListingService.Infra.Messaging;
using ListingService.Infra.Messaging.Services;
using ListingService.Infra.Persistence.Repositories;
using ListingService.Infra.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polly;
using Polly.Extensions.Http;

namespace ListingService.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region MongoDB
        var dbName = "listingdb";
        var connectionString = configuration["MONGODB_CONNECTION_STRING"];

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Environment Variable MONGODB_CONNECTION_STRING not found.");

        services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));
        #endregion

        #region MassTransit - RabbitMQ
        services.AddMassTransit(busConfigurator =>
        {
            // Timeout Consumers
            busConfigurator.AddConsumer<BuyPendingTimeoutConsumer>();
            busConfigurator.AddConsumer<NewBidPendingTimeoutConsumer>();

            // Auction Status Consumers
            busConfigurator.AddConsumer<StartAuctionConsumer>();
            busConfigurator.AddConsumer<SetAuctionAsEndingConsumer>();
            busConfigurator.AddConsumer<FinishAuctionConsumer>();

            // Payments Consumers
            busConfigurator.AddConsumer<BidPaymentFailedConsumer>();
            busConfigurator.AddConsumer<BidPaymentSucceededConsumer>();
            busConfigurator.AddConsumer<PurchasePaymentFailedConsumer>();
            busConfigurator.AddConsumer<PurchasePaymentSucceededConsumer>();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration["RABBITMQ_CONNECTION_STRING"];

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Environment Variable RABBITMQ_CONNECTION_STRING not found.");

                cfg.Host(new Uri(connectionString));

                cfg.UseDelayedMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
        });
        #endregion

        #region Http Client for Product Service
        var productServiceUrl = Environment.GetEnvironmentVariable("PRODUCT_SERVICE_URL")
            ?? throw new InvalidOperationException("PRODUCT_SERVICE_URL não definido no .env");

        services.AddHttpClient<IProductServiceClient, HttpProductServiceClient>(client =>
        {
            client.BaseAddress = new Uri(productServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(3);
        })
            .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(1)));
        #endregion

        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IListingRepository, ListingRepository>();
        services.AddScoped<IMessageBus, MassTransitMessageBus>();
        services.AddScoped<IListingReadModelPublisher, ListingReadModelPublisher>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}