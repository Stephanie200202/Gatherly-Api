namespace Gatherly.Application.DTOs.Events
{
    public class EventListItemDto
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string? BannerUrl { get; set; }
        public int Capacity { get; set; }
        public int RegisteredCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
    }
}
