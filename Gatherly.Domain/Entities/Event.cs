using System;

namespace Gatherly.Domain.Entities
{
    public class Event
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string? EndTime { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public string Visibility { get; set; } = "Public";
        public bool AllowReEntry { get; set; }
        public bool VipEnabled { get; set; }
        public string? RsvpDeadline { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Published, RegistrationClosed, Cancelled
        public Guid OrganizerId { get; set; }
        public ApplicationUser Organizer { get; set; }
        public string? BannerUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}