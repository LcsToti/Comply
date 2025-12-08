using MassTransit;
using MediatR;
using NotificationService.Domain.Contracts;
using Shared.Contracts.Messages.NotificationService;

namespace NotificationService.App.Commands.WatchList.AddToWatchList;

public record AddToWatchListCommandHandler : IRequestHandler<AddToWatchListCommand>
{
    private readonly IWatchListRepository _watchListRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public AddToWatchListCommandHandler(IWatchListRepository watchListRepository, IPublishEndpoint publishEndpoint)
    {
        _watchListRepository = watchListRepository;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Handle(AddToWatchListCommand request, CancellationToken cancellationToken)
    {
        //App
        
        var watchList = await _watchListRepository.GetByUserIdAsync(request.UserId);
        
        if (watchList == null)
        {
            watchList = Domain.Entities.WatchList.Create(request.UserId);
        }
        
        //Domain
        watchList.AddToWatchList(request.ProductId);
        //Persist
        await _watchListRepository.SaveAsync(watchList);
        //Message Bus
        await _publishEndpoint.Publish(new IncrementWatchListMessage(request.ProductId), cancellationToken);
    }
}