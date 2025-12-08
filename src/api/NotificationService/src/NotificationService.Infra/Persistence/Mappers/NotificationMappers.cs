using NotificationService.Domain.Entities;
using NotificationService.Infra.Persistence.DataModel;

namespace NotificationService.Infra.Persistence.Mappers;

public static class NotificationMappers
{
    public static NotificationDataModel ToDataModel(this Notification notification)
    {
        return new NotificationDataModel()
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Message = notification.Message,
            Read = notification.Read,
            CreatedAt = notification.CreatedAt,
        };
    }

    public static Notification ToDomain(this NotificationDataModel dm)
    {
        return Notification.Load(
            dm.Id,
            dm.UserId,
            dm.Type,
            dm.Message,
            dm.Read,
            dm.CreatedAt
            );
    }
}