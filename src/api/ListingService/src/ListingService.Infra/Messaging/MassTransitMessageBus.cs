using ListingService.App.Common.Interfaces;
using MassTransit;

namespace ListingService.Infra.Messaging;

public class MassTransitMessageBus(IPublishEndpoint publishEndpoint) : IMessageBus
{
    public async Task PublishAsync<T>(
        T message,
        CancellationToken cancellationToken) where T : class
    {
        await publishEndpoint.Publish(message, cancellationToken);
    }

    public async Task PublishAsync<T>(
        T message,
        Action<IPublishOptions> configure,
        CancellationToken cancellationToken) where T : class
    {
        var options = new PublishOptions();

        configure.Invoke(options);

        await publishEndpoint.Publish(message, ctx =>
        {
            if (options.Delay is TimeSpan delay)
                ctx.Delay = delay;

            if (options.TimeToLive is TimeSpan ttl)
                ctx.TimeToLive = ttl;

        }, cancellationToken);
    }

    private sealed class PublishOptions : IPublishOptions
    {
        public TimeSpan? Delay { get; set; }
        public TimeSpan? TimeToLive { get; set; }
    }
}