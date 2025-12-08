using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Contracts
{
    public interface ITicketRepository
    {
        Task SaveAsync(SupportTicket ticket);
        Task UpdateAsync(SupportTicket ticket);
        Task<List<SupportTicket>> GetAllAsync();
        Task<SupportTicket?> GetByIdAsync(Guid id);
        Task<IEnumerable<SupportTicket>> GetByUserIdAsync(Guid userId);
    }
}
