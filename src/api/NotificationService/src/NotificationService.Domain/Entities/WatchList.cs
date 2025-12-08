namespace NotificationService.Domain.Entities;

public class WatchList
{
    public Guid UserId { get; private set; }
    public List<Guid> ProductsWatching { get; private set; }

    private WatchList() { }
    private WatchList(Guid userId)
    {
        UserId = userId;
        ProductsWatching = new List<Guid>();
    }

    public static WatchList Create(Guid userId)
    {
        return new WatchList(userId);
    }
    public static WatchList Load(Guid userId, List<Guid>? productsWatching)
    {
        return new WatchList
        {
            UserId = userId,
            ProductsWatching = productsWatching ?? new List<Guid>()
        };
    }
    public void AddToWatchList(Guid productId)
    {
        if (!ProductsWatching.Contains(productId))
        {
            ProductsWatching.Add(productId);
        }
    }
    public void RemoveFromWatchList(Guid productId)
    {
        ProductsWatching.Remove(productId);
    }
}