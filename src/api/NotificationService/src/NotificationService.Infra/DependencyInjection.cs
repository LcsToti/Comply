using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NotificationService.Domain.Contracts;
using NotificationService.Infra.MessageBroker.Consumers.Auction;
using NotificationService.Infra.MessageBroker.Consumers.Bids;
using NotificationService.Infra.MessageBroker.Consumers.Payments.BidPayments;
using NotificationService.Infra.MessageBroker.Consumers.Payments.Payouts;
using NotificationService.Infra.MessageBroker.Consumers.Payments.PurchasePayments;
using NotificationService.Infra.MessageBroker.Consumers.Product;
using NotificationService.Infra.Persistence.Repositories;

namespace NotificationService.Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            #region MongoDB
            var dbName = "notificationdb";
            var connectionString = configuration["MONGODB_CONNECTION_STRING"];

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Environment Variable MONGODB_CONNECTION_STRING not found.");

            services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
            services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));
            #endregion

            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IWatchListRepository, WatchListRepository>();

            #region RabbitMQ
            services.AddMassTransit(rabbitmq =>
            {
                // Consumers
                rabbitmq.AddConsumer<AuctionEndedConsumer>();
                rabbitmq.AddConsumer<AuctionExtendedConsumer>();
                rabbitmq.AddConsumer<AuctionEndingConsumer>();
                rabbitmq.AddConsumer<AuctionStartConsumer>();

                rabbitmq.AddConsumer<BidOutbiddedConsumer>();
                rabbitmq.AddConsumer<BidPlacedConsumer>();

                rabbitmq.AddConsumer<BidPaymentFailedConsumer>();
                rabbitmq.AddConsumer<BidPaymentRefundedConsumer>();
                rabbitmq.AddConsumer<BidPaymentSucceededConsumer>();
                rabbitmq.AddConsumer<ApprovedToWithdrawalConsumer>();
                rabbitmq.AddConsumer<PurchasePaymentFailedConsumer>();
                rabbitmq.AddConsumer<PurchasePaymentRefundedConsumer>();
                rabbitmq.AddConsumer<PurchasePaymentSucceededConsumer>();

                rabbitmq.AddConsumer<ProductBoughtConsumer>();


                rabbitmq.UsingRabbitMq((context, cfg) =>
                {
                    var connectionString = configuration["RABBITMQ_CONNECTION_STRING"];

                    if (string.IsNullOrEmpty(connectionString))
                        throw new InvalidOperationException("Environment Variable RABBITMQ_CONNECTION_STRING not found.");

                    cfg.Host(new Uri(connectionString));

                    // --- Auction events ---
                    cfg.ReceiveEndpoint("auction-ended-notification",
                        e => { e.ConfigureConsumer<AuctionEndedConsumer>(context); });

                    cfg.ReceiveEndpoint("auction-extended-notification",
                        e => { e.ConfigureConsumer<AuctionExtendedConsumer>(context); });

                    cfg.ReceiveEndpoint("auction-ending-notification",
                        e => { e.ConfigureConsumer<AuctionEndingConsumer>(context); });

                    cfg.ReceiveEndpoint("auction-start-notification",
                        e => { e.ConfigureConsumer<AuctionStartConsumer>(context); });


                    // --- Bid events ---
                    cfg.ReceiveEndpoint("bid-outbidded-notification",
                        e => { e.ConfigureConsumer<BidOutbiddedConsumer>(context); });

                    cfg.ReceiveEndpoint("bid-placed-notification",
                        e => { e.ConfigureConsumer<BidPlacedConsumer>(context); });


                    // --- Bid payment events ---
                    cfg.ReceiveEndpoint("bid-payment-failed-notification",
                        e => { e.ConfigureConsumer<BidPaymentFailedConsumer>(context); });

                    cfg.ReceiveEndpoint("bid-payment-refunded-notification",
                        e => { e.ConfigureConsumer<BidPaymentRefundedConsumer>(context); });

                    cfg.ReceiveEndpoint("bid-payment-succeeded-notification",
                        e => { e.ConfigureConsumer<BidPaymentSucceededConsumer>(context); });


                    // --- Purchase payments ---
                    cfg.ReceiveEndpoint("purchase-payment-failed-notification",
                        e => { e.ConfigureConsumer<PurchasePaymentFailedConsumer>(context); });

                    cfg.ReceiveEndpoint("purchase-payment-refunded-notification",
                        e => { e.ConfigureConsumer<PurchasePaymentRefundedConsumer>(context); });

                    cfg.ReceiveEndpoint("purchase-payment-succeeded-notification",
                        e => { e.ConfigureConsumer<PurchasePaymentSucceededConsumer>(context); });


                    // --- Withdrawal ---
                    cfg.ReceiveEndpoint("approved-to-withdrawal-notification",
                        e => { e.ConfigureConsumer<ApprovedToWithdrawalConsumer>(context); });


                    // --- Product bought ---
                    cfg.ReceiveEndpoint("product-bought-notification",
                        e => { e.ConfigureConsumer<ProductBoughtConsumer>(context); });


                    // Criar endpoints automáticos para qualquer consumer não mapeado
                    cfg.ConfigureEndpoints(context);
                });
            });
            #endregion

            return services;
        }
    }
}