using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Payments.App.Common.Contracts;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Contracts;
using Payments.Infra.MessageBroker.Consumers;
using Payments.Infra.MessageBroker.Publishers;
using Payments.Infra.PaymentGateway;
using Payments.Infra.PaymentGateway.Exceptions;
using Payments.Infra.Persistence.Repository;

namespace Payments.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        #region MongoDb
        var dbName = "paymentdb";
        var connectionString = configuration["MONGODB_CONNECTION_STRING"];

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Environment Variable MONGODB_CONNECTION_STRING not found.");

        services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));
        #endregion

        #region MassTransit
        services.AddMassTransit(rabbitmq =>
        {
            rabbitmq.AddConsumer<ApproveWithdrawalConsumerService>();
            rabbitmq.AddConsumer<CreatePaymentAccountConsumerService>();
            rabbitmq.AddConsumer<PurchaseRefundConsumerService>();
            rabbitmq.AddConsumer<BidRefundConsumerService>();
            rabbitmq.AddConsumer<ProcessPurchasePaymentConsumerService>();
            rabbitmq.AddConsumer<ProcessBidPaymentConsumerService>();
            rabbitmq.AddConsumer<DisputeRefundConsumerService>();

            rabbitmq.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration["RABBITMQ_CONNECTION_STRING"];

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Environment Variable RABBITMQ_CONNECTION_STRING not found.");

                cfg.Host(new Uri(connectionString));

                cfg.ReceiveEndpoint("enable-withdraw", e =>
                {
                    e.ConfigureConsumer<ApproveWithdrawalConsumerService>(context);
                });
                cfg.ReceiveEndpoint("user-created", e =>
                {
                    e.ConfigureConsumer<CreatePaymentAccountConsumerService>(context);
                });
                cfg.ReceiveEndpoint("process-purchase-refund", e =>
                {
                    e.ConfigureConsumer<PurchaseRefundConsumerService>(context);
                });
                cfg.ReceiveEndpoint("process-bid-refund", e =>
                {
                    e.ConfigureConsumer<BidRefundConsumerService>(context);
                });
                cfg.ReceiveEndpoint("process-purchase", e =>
                {
                    e.ConfigureConsumer<ProcessPurchasePaymentConsumerService>(context);
                });
                cfg.ReceiveEndpoint("process-new-bid", e =>
                {
                    e.ConfigureConsumer<ProcessBidPaymentConsumerService>(context);
                });
                cfg.ReceiveEndpoint("refund-dispute", e =>
                {
                    e.ConfigureConsumer<DisputeRefundConsumerService>(context);
                });


                cfg.ConfigureEndpoints(context);
            });
        });
        #endregion

        #region StripeGateway ApiKey
        var apiKey = configuration["StripeConfiguration:ApiKey"];
        var refreshUrl = configuration["StripeConfiguration:RefreshUrl"];
        var returnUrl = configuration["StripeConfiguration:ReturnUrl"];
        if (string.IsNullOrEmpty(apiKey))
            throw new PaymentGatewayConnectionException("Required Stripe setting 'ApiKey' was not found or is empty in the configuration.");
        if (string.IsNullOrEmpty(refreshUrl))
            throw new PaymentGatewayConnectionException("Required Stripe setting 'RefreshUrl' was not found or is empty in the configuration.");
        if (string.IsNullOrEmpty(returnUrl))
            throw new PaymentGatewayConnectionException("Required Stripe setting 'ReturnUrl' was not found or is empty in the configuration.");
        #endregion

        services.AddScoped<IPaymentRepository<Payment>, PaymentRepository<Payment>>();
        services.AddScoped<IPaymentAccountRepository<PaymentAccount>, PaymentAccountRepository<PaymentAccount>>();
        services.AddScoped<IPaymentGateway, StripePaymentGateway>();
        services.AddScoped<IPaymentPublisher, PurchasePaymentResultPublisher>();
        services.AddScoped<IPaymentPublisher, BidPaymentResultPublisher>();

        return services;
    }
}