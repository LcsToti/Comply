namespace NotificationService.Domain.VOs
{
    public class Comment
    {
        public Guid AuthorId { get; private set; }
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }
        
        private Comment() { }
        private Comment(Guid authorId, string content, DateTime createdAt)
        {
            AuthorId = authorId;
            Content = content;
            CreatedAt = createdAt;
        }
        public static Comment Create(Guid authorId, string content)
        {
            if (authorId == Guid.Empty)
                throw new ArgumentException("AuthorId cannot be empty.", nameof(authorId));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Comment content cannot be empty or null.", nameof(content));

            if (content.Length > 2000)
                throw new ArgumentException("Comment content cannot exceed 2000 characters.", nameof(content)); 

            return new Comment(authorId, content, DateTime.UtcNow);
        }
        public static Comment Load(Guid authorId, string content, DateTime createdAt)
        {
            return new Comment()
            {
                AuthorId = authorId,
                Content = content,
                CreatedAt = createdAt
            };
        }
    }
}