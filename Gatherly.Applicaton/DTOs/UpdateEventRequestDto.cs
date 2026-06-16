using Gatherly.Application.DTOs.Events;
using System.ComponentModel.DataAnnotations;

namespace Gatherly.Application.DTOs.Events
{
    public class UpdateEventRequestDto
    {
        [MaxLength(100)]
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? Venue { get; set; }
        public string? Date { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        public int? Capacity { get; set; }
        public string? Visibility { get; set; }
        public bool? AllowReEntry { get; set; }
        public bool? VipEnabled { get; set; }
        public string? RsvpDeadline { get; set; }
    }
}
