using Gatherly.Domain.Entities;
using System;

namespace Gatherly.Domain.Entities
{
    public class Pass
    {
        public Guid PassId { get; set; } = Guid.NewGuid();
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventVenue { get; set; } = string.Empty;
        public string AttendeeName { get; set; } = string.Empty;
        public string TicketType { get; set; } = "General"; // e.g., General, VIP [cite: 20]
        public string Status { get; set; } = "Active"; // Active, Used, Expired, Invalidated, Flagged [cite: 33]
        public string QrPayload { get; set; } = string.Empty;
        public DateTime TokenExpiresAt { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public Guid RegistrationId { get; set; }
    }
}