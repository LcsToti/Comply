using MassTransit;
using MediatR;
using NotificationService.Domain.Contracts;
using Shared.Contracts.Messages.NotificationService;

namespace NotificationService.App.Commands.WatchList.RemoveFromWatchList;

public record RemoveFromWatchListCommandHandler : IRequestHandler<RemoveFromWatchListCommand>
{
    private readonly IWatchListRepository _watchListRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public RemoveFromWatchListCommandHandler(IWatchListRepository watchListRepository, IPublishEndpoint publishEndpoint)
    {
        _watchListRepository = watchListRepository;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Handle(RemoveFromWatchListCommand request, CancellationToken cancellationToken)
    {
        //App
        
        var watchList = await _watchListRepository.GetByUserIdAsync(request.UserId);

        if (watchList == null)
        {
            return;
        }
        
        //Domain
        watchList.RemoveFromWatchList(request.ProductId);
        
        //Persist
        await _watchListRepository.SaveAsync(watchList);
        
        //Message Bus
        await _publishEndpoint.Publish(new DecrementWatchListMessage(request.ProductId), cancellationToken);
    }
}