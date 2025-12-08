using NotificationService.Domain.Entities;
using NotificationService.Infra.Persistence.DataModel;

namespace NotificationService.Infra.Persistence.Mappers;

public static class WatchListMappers
{
    public static WatchListDataModel ToDataModel(this WatchList model)
    {
        return new WatchListDataModel
        {
            UserId = model.UserId,
            ProductsWatching = model.ProductsWatching
        };
    }

    public static WatchList ToDomain(this WatchListDataModel dataModel)
    {
        return WatchList.Load(
            dataModel.UserId,
            dataModel.ProductsWatching
        );
    }
}