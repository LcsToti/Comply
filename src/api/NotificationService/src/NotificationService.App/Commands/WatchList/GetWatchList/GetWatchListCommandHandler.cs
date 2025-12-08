using MediatR;
using NotificationService.Domain.Contracts;

namespace NotificationService.App.Commands.WatchList.GetWatchList;

public class GetWatchListCommandHandler(IWatchListRepository watchListRepository) : IRequestHandler<GetWatchListCommand, List<Guid>>
{
    private readonly IWatchListRepository _watchListRepository = watchListRepository;

    public async Task<List<Guid>> Handle(GetWatchListCommand request, CancellationToken cancellationToken)
    {
        var watchListEntity = await _watchListRepository.GetByUserIdAsync(request.UserId);

        if (watchListEntity is null) return [];

        return [.. watchListEntity.ProductsWatching];
    }
}