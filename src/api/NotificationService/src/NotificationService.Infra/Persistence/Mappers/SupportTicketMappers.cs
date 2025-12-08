using NotificationService.Domain.Entities;
using NotificationService.Domain.VOs;
using NotificationService.Infra.Persistence.DataModel;

namespace NotificationService.Infra.Persistence.Mappers;

public static class SupportTicketMappers
{
    public static SupportTicketDataModel ToDataModel(this SupportTicket model)
    {
            return new SupportTicketDataModel
            {
                Id = model.Id,
                UserId = model.UserId,
                Title = model.Title,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                Status = model.Status,
                LastUpdateAt = model.LastUpdateAt,
                AssignedToAdminId = model.AssignedToAdminId,
                _comments = model.Comments
                    .Select(c => new CommentDataModel
                    {
                        AuthorId = c.AuthorId,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt
                    })
                    .ToList()
            };
        
    }
    
    public static SupportTicket ToDomain(this SupportTicketDataModel dataModel)
    {
        var comments = dataModel.Comments?
            .Select(c => Comment.Load(c.AuthorId, c.Content, c.CreatedAt))
            .ToList();

        return SupportTicket.Load(
            id: dataModel.Id,
            userId: dataModel.UserId,
            title: dataModel.Title,
            description: dataModel.Description,
            createdAt: dataModel.CreatedAt,
            status: dataModel.Status,
            lastUpdateAt: dataModel.LastUpdateAt,
            assignedToAdminId: dataModel.AssignedToAdminId,
            comments: comments
        );
    }
}