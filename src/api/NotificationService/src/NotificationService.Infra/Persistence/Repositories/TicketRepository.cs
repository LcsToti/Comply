using MongoDB.Driver;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;
using NotificationService.Infra.Persistence.DataModel;
using NotificationService.Infra.Persistence.Mappers;

namespace NotificationService.Infra.Persistence.Repositories;

public class TicketRepository(IMongoDatabase database) : ITicketRepository
{
    private readonly IMongoCollection<SupportTicketDataModel> _collection =
        database.GetCollection<SupportTicketDataModel>("SupportTickets");

    public async Task<List<SupportTicket>> GetAllAsync()
    {
        var tickets = await _collection.Find(_ => true).ToListAsync();
        return [.. tickets.Select(t => t.ToDomain())];
    }

    public async Task<SupportTicket?> GetByIdAsync(Guid id)
    {
        var ticket = await _collection.Find(t => t.Id == id).FirstOrDefaultAsync();
        return ticket.ToDomain();
    }

    public async Task SaveAsync(SupportTicket ticket)
    {
        var ticketDataModel = ticket.ToDataModel();
        var options = new ReplaceOptions { IsUpsert = true };

        await _collection.ReplaceOneAsync(
            filter: t => t.Id == ticket.Id,
            replacement: ticketDataModel,
            options
        );
    }

    public Task UpdateAsync(SupportTicket ticket)
    {
        return SaveAsync(ticket);
    }

    public async Task<IEnumerable<SupportTicket>> GetByUserIdAsync(Guid userId)
    {
        var tickets = await _collection
            .Find(t => t.UserId == userId)
            .SortByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tickets.Select(t => t.ToDomain());
    }
}