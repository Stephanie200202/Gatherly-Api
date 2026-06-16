using System;

namespace Gatherly.Domain.Entities
{
    public class Notification
    {
        public Guid NotificationId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Type { get; set; } = string.Empty; // RegistrationConfirmed, EventReminder, OrganizerAnnouncement, etc. [cite: 44]
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public Guid RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}