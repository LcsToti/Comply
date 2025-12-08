namespace ListingService.App.Common.Interfaces;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, Action<IPublishOptions> options, CancellationToken cancellationToken) where T : class;
    Task PublishAsync<T>(T message, CancellationToken cancellationToken) where T : class;
}

public interface IPublishOptions
{
    TimeSpan? Delay { get; set; }
    TimeSpan? TimeToLive { get; set; }
}