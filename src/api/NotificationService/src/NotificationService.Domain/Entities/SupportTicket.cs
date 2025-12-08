using NotificationService.Domain.Enums;
using NotificationService.Domain.VOs;

namespace NotificationService.Domain.Entities
{
    public class SupportTicket
    {
        public Guid Id {  get; private set; }
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public TicketStatus Status { get; private set; }
        public DateTime LastUpdateAt { get; private set; }
        public Guid? AssignedToAdminId { get; private set; }

        private List<Comment> _comments = new List<Comment>();
        public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();
        private SupportTicket(Guid userId, string title, string description)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            LastUpdateAt = DateTime.UtcNow;
            Status = TicketStatus.Open;
            AssignedToAdminId = null;
        }
        
        private SupportTicket() { }
        public static SupportTicket Create(Guid userId, string title, string description)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Subject cannot be empty.", nameof(title));
            if (title.Length > 100)
                throw new ArgumentException("Title cannot exceed 100 characters.", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.", nameof(description));
            if (description.Length > 1000)
                throw new ArgumentException("Description cannot exceed 1000 characters.", nameof(description));
            if (description.Length < 10)
                throw new ArgumentException("Description must be at least 10 characters long.", nameof(description));
            return new SupportTicket(userId, title, description);
        }
        public static SupportTicket Load(
            Guid id,
            Guid userId,
            string title,
            string description,
            DateTime createdAt,
            TicketStatus status,
            DateTime lastUpdateAt,
            Guid? assignedToAdminId,
            IEnumerable<Comment>? comments = null)
        {
            var ticket = new SupportTicket
            {
                Id = id,
                UserId = userId,
                Title = title,
                Description = description,
                CreatedAt = createdAt,
                Status = status,
                LastUpdateAt = lastUpdateAt,
                AssignedToAdminId = assignedToAdminId,
                _comments = comments?.ToList() ?? new List<Comment>()
            };

            return ticket;
        }
        
        public void AddComment(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment), "Comment object cannot be null.");
            
            _comments.Add(comment);

            if (Status == TicketStatus.Open)
            {
                Status = TicketStatus.InProgress;
            }

            LastUpdateAt = DateTime.UtcNow;
        }
        public void AssignToAdmin(Guid adminId)
        {
            if (adminId == Guid.Empty) throw new ArgumentException("Admin ID is required.");
            if (Status == TicketStatus.Closed) throw new InvalidOperationException("Cannot assign a closed ticket.");

            AssignedToAdminId = adminId;
            Status = TicketStatus.InProgress;
            LastUpdateAt = DateTime.UtcNow;
        }
        public void UpdateStatus(TicketStatus newStatus)
        {
            if (Status == TicketStatus.Closed && newStatus != TicketStatus.Closed)
                throw new InvalidOperationException("Cannot reopen a closed ticket.");

            Status = newStatus;
            LastUpdateAt = DateTime.UtcNow;
        }
    }
}
