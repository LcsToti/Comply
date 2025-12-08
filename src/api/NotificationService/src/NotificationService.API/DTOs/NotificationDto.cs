public class NotificationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string SourceType { get; set; }
    public Guid SourceId { get; set; }
}