namespace ProductService.Application.Contracts;

public interface IMassTransitMessageBus
{
    Task PublishAsync<T>(T message, Action<IPublishOptions>? options = null, CancellationToken cancellationToken = default) where T : class;
}

public interface IPublishOptions
{
    TimeSpan? Delay { get; set; }
    TimeSpan? TimeToLive { get; set; }
}