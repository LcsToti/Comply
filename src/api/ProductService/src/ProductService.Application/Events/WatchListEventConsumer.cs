using ProductService.Application.Enums;

namespace ProductService.Application.Events;

public record WatchListEventConsumer(Guid ProductId, WatchListActionEvent Action);