using System;

namespace Gatherly.Application.DTOs.Events
{
    public class EventDetailsResponseDto
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string? EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; }
        public int AvailableSpots { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Visibility { get; set; } = string.Empty;
        public bool AllowReEntry { get; set; }
        public bool VipEnabled { get; set; }
        public string? RsvpDeadline { get; set; }
        public string? BannerUrl { get; set; }
        public OrganizerDto Organizer { get; set; } = new OrganizerDto();
        public DateTime CreatedAt { get; set; }
    }
}