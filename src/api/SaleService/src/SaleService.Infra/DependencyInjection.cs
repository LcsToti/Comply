
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SalesService.App.Common.Contracts;
using SalesService.Domain.Aggregates.SaleAggregate.Entities;
using SalesService.Domain.Contracts;
using SalesService.Infra.MessageBroker.Consumers;
using SalesService.Infra.MessageBroker.Publishers;
using SalesService.Infra.Persistence.Repository;

namespace SalesService.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region MongoDB
        var dbName = "saledb";
        var connectionString = configuration["MONGODB_CONNECTION_STRING"];

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Environment Variable MONGODB_CONNECTION_STRING not found.");

        services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));
        #endregion

        #region MassTransit - RabbitMQ
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<CloseDisputeAsExpiredConsumer>();
            busConfigurator.AddConsumer<CreateSaleConsumer>();

            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration["RABBITMQ_CONNECTION_STRING"];

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Environment Variable RABBITMQ_CONNECTION_STRING not found.");

                cfg.Host(new Uri(connectionString));

                cfg.UseDelayedMessageScheduler();

                cfg.ReceiveEndpoint("open_dispute_timeout_queue", e =>
                {
                    e.ConfigureConsumer<CloseDisputeAsExpiredConsumer>(context);
                });

                cfg.ReceiveEndpoint("create_sale", e =>
                {
                    e.ConfigureConsumer<CreateSaleConsumer>(context);
                });


                cfg.ConfigureEndpoints(context);
            });
        });
        #endregion


        services.AddScoped<ISaleRepository<Sale>, SaleRepository>();
        services.AddScoped<ISalePublisher, SalePublisher>();

        return services;
    }
}