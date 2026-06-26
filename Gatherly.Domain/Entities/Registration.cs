using System;

namespace Gatherly.Domain.Entities
{
    public class Registration
    {
        public Guid RegistrationId { get; set; } = Guid.NewGuid();
        public Guid EventId { get; set; }
        public Guid? UserId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        
        public string TicketType { get; set; } = "Regular"; // Regular, VIP
        public string Status { get; set; } = "Confirmed";   // Confirmed, Cancelled, CheckedIn
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public DateTime? CheckedInAt { get; set; }
        public decimal AmountPaid { get; set; }
        public string? PaymentReference { get; set; }
    }
}