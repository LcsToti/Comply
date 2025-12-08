using DotNet.Testcontainers.Builders;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;

namespace ProductsService.Integration.Tests.Fixture;

public class ContainersFixture : IAsyncLifetime
{
    public MongoDbContainer MongoContainer { get; private set; } = default!;
    public RabbitMqContainer RabbitMqContainer { get; private set; } = default!;

    public string MongoConnection => MongoContainer.GetConnectionString();
    public string RabbitMqConnection => RabbitMqContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        MongoContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .WithName("mongo-test-container")
            .WithUsername("testUser")
            .WithPassword("testPassword")
            .Build();

        RabbitMqContainer = new RabbitMqBuilder()
            .WithImage("heidiks/rabbitmq-delayed-message-exchange:latest")
            .WithUsername("guest")
            .WithPassword("guest")
            .WithPortBinding(RabbitMqBuilder.RabbitMqPort, true)
            .Build();

        await Task.WhenAll(
            MongoContainer.StartAsync(),
            RabbitMqContainer.StartAsync());
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
             MongoContainer.DisposeAsync().AsTask(),
             RabbitMqContainer.DisposeAsync().AsTask());
    }
}