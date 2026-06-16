using System;

namespace Gatherly.Domain.Entities
{
    public class Announcement
    {
        public Guid AnnouncementId { get; set; } = Guid.NewGuid();
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium"; // High, Medium, Low [cite: 45]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}