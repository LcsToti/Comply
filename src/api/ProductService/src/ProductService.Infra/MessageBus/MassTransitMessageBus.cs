using MassTransit;
using ProductService.Application.Contracts;

namespace ProductService.Infrastructure.MessageBus;

public class MassTransitMessageBus(IPublishEndpoint publishEndpoint) : IMassTransitMessageBus
{
    public async Task PublishAsync<T>(
        T message,
        Action<IPublishOptions>? configure = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var options = new PublishOptions();
        configure?.Invoke(options);

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
