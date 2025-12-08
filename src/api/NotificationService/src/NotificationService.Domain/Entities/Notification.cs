using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public NotificationType Type { get; private set; }
        public string Message { get; private set; }
        public bool Read { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Notification() { }
        private Notification(Guid userId, NotificationType type, string message)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Type = type;
            Message = message;
            Read = false;
            CreatedAt = DateTime.UtcNow;
        }
        public static Notification Create(Guid userId, NotificationType type, string message)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.", nameof(message));
            if (message.Length > 500)
                throw new ArgumentException("Message cannot exceed 500 characters.", nameof(message));
            return new Notification(userId, type, message);
        }
        public static Notification Load(Guid id, Guid userId, NotificationType type, string message, bool read, DateTime createdAt)
        {
            return new Notification
            {
                Id = id,
                UserId = userId,
                Type = type,
                Message = message,
                Read = read,
                CreatedAt = createdAt
            };
        }
        public void MarkAsRead()
        {
            if (Read)
                return;
            
            Read = true;
        }
    }
}
