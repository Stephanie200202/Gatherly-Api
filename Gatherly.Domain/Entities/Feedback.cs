using System;

namespace Gatherly.Domain.Entities
{
    public class Feedback
    {
        public Guid FeedbackId { get; set; } = Guid.NewGuid();
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; } // 1 to 5 Stars
        public string Comment { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
